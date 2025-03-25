using AutoMapper;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Helper;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RentEase.Service.Service.Payment
{

    public interface IPayosService
    {
        Task<ServiceResult> CheckOut(OrderReq request);
        Task<ServiceResult> Callback(PaymentCallback request);
    }
    public class PayosService : IPayosService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ServiceWrapper _serviceWrapper;
        private readonly HelperWrapper _helperWrapper;
        public PayosService(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IConfiguration configuration,
                                           IMapper mapper, ServiceWrapper serviceWrapper, HelperWrapper helperWrapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _unitOfWork ??= new UnitOfWork();
            _configuration = configuration;
            _mapper = mapper;
            _serviceWrapper = serviceWrapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> CheckOut(OrderReq request)
        {

            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tài khoản không tồn tại");
            }
            string orderCode = "";
            do
            {
                orderCode = $"{GenerateOrderCode()}{DateTime.Now:HHmmssfff}";
            }
            while (await _unitOfWork.OrderRepository.GetByOrderCodeAsync(orderCode) != null);

            var createItem = new Order()
            {
                OrderId = Guid.NewGuid().ToString("N"),
                OrderTypeId = request.OrderTypeId,
                OrderCode = orderCode,
                PostId = request.PostId,
                SenderId = accountId,
                TotalAmount = request.Amount + (request.Amount * request.IncurredCost),
                Note = request.Note,
                CreatedAt = DateTime.Now,
                PaymentStatusId = (int)EnumType.ApproveStatusId.Pending
            };

            var result = await _unitOfWork.OrderRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var order = await _unitOfWork.OrderRepository.GetByOrderCodeAsync(createItem.OrderCode);
                var orderRes = _mapper.Map<OrderRes>(order);

                string jsonResponse = await this.CreatePaymentURL(createItem.OrderCode);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Bỏ qua phân biệt chữ hoa/thường
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua giá trị null
                };

                PayosRes payosRes = JsonSerializer.Deserialize<PayosRes>(jsonResponse, options);

                if (payosRes == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tạo link thanh toán thất bại");
                }

                var responseData = new PaymentRes()
                {
                    OrderRes = orderRes,
                    PayosRes = payosRes,
                };
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo link thanh toán thành công", responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tạo link thanh toán thất bại");
        }

        public async Task<ServiceResult> Callback(PaymentCallback request)
        {
            var order = await _unitOfWork.OrderRepository.GetByOrderCodeAsync(request.OrderCode);
            if (order == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tìm thấy đơn hàng");
            }

            order.PaymentStatusId = request.Status switch
            {
                "PAID" => (int)EnumType.PaymentStatusId.PAID,
                "PENDING" => (int)EnumType.PaymentStatusId.PENDING,
                "PROCESSING" => (int)EnumType.PaymentStatusId.PROCESSING,
                "CANCELLED" => (int)EnumType.PaymentStatusId.CANCELLED,
            };

            if (order.PaymentStatusId == (int)EnumType.PaymentStatusId.PAID)
            {
                order.PaidAt = DateTime.Now;

                var orderType = await _unitOfWork.OrderTypeRepository.GetByIdAsync(order.OrderTypeId);
                var post = await _unitOfWork.PostRepository.GetByIdAsync(order.PostId);
                post.StartPublic = DateTime.Now;
                post.EndPublic = DateTime.Now.AddMonths(orderType.Month);
                post.UpdatedAt = DateTime.Now;
                post.Status = true;
            }
            else
            {
                order.CancelledAt = DateTime.Now;
            }


            await _unitOfWork.OrderRepository.UpdateAsync(order);

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cap nhat trang thai don hang thanh cong");
        }



        private async Task<string> CreatePaymentURL(string orderCode)
        {
            var requestUrl = _configuration["PayosSettings:RequestUrl"];
            var clientId = _configuration["PayosSettings:ClientId"];
            var apiKey = _configuration["PayosSettings:ApiKey"];
            var checksumKey = _configuration["PayosSettings:ChecksumKey"];

            var returnUrl = _configuration["PayosSettings:ReturnUrl"];
            var cancelUrl = _configuration["PayosSettings:CancelUrl"];

            var request = await _unitOfWork.OrderRepository.GetByOrderCodeAsync(orderCode);

            // Tạo dictionary chứa tham số
            var payload = new Dictionary<string, object>
                             {
                                 { "amount", (int)request.TotalAmount },
                                 { "cancelUrl", cancelUrl },
                                 { "description", request.Note },
                                 { "orderCode", long.Parse(request.OrderCode) },
                                 { "returnUrl", returnUrl }
                             };

            // Sắp xếp tham số và tạo chuỗi signature
            string rawData = PayOSUtils.CreateSortedQueryString(payload);
            string signature = PayOSUtils.GenerateHmacSha256(rawData, checksumKey!);

            payload["signature"] = signature;


            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
            try
            {
                var response = await _httpClient.PostAsync(requestUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Lỗi từ PayOS: {response.StatusCode}, Nội dung: {responseString}");
                }

                return responseString;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi gọi API PayOS: " + ex.Message);
            }
        }
        private static string GenerateOrderCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Sinh số ngẫu nhiên từ 100000 đến 999999
        }
    }
}
