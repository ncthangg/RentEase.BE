using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentEase.Common.Base;
using RentEase.Common.DTOs;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Helper;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static RentEase.Common.Base.EnumType;

namespace RentEase.Service.Service.Payment
{

    public interface IPayosService
    {
        Task<ServiceResult> CheckOut(TransactionReq request);
        Task<ServiceResult> Callback(PaymentCallback request);
        //VnPayRes PaymentExecute(IQueryCollection collections);
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

        public async Task<ServiceResult> CheckOut(TransactionReq request)
        {

            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tài khoản không tồn tại");
            }

            var transactionItem = await _unitOfWork.TransactionRepository.GetByOrderCode(request.OrderId);
            var orderItem = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);

            if (orderItem.PaymentStatusId == (int)EnumType.PaymentStatusId.PAID ||
                        orderItem.PaymentStatusId == (int)EnumType.PaymentStatusId.PROCESSING ||
                               orderItem.PaymentStatusId == (int)EnumType.PaymentStatusId.CANCELLED)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đơn hàng đã thanh toán hoặc đã được hủy");
            }

            if (orderItem.SenderId != accountId)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đơn hàng không thuộc tài khoản này");
            }

            //Khai báo
            TransactionRes transactionRes = new TransactionRes();
            PayosRes payosRes = new PayosRes();

            if (transactionItem != null)
            {
                // Tăng giá trị lên 1 rồi chuyển lại thành string
                var newPaymentAttempt = transactionItem.PaymentAttempt + 1;

                var createItem = new Transaction()
                {
                    TransactionTypeId = orderItem.TransactionTypeId,
                    OrderId = orderItem.OrderId,
                    PaymentAttempt = newPaymentAttempt,
                    PaymentCode = $"{orderItem.OrderId}{DateTime.Now:HHmmssfff}",
                    TotalAmount = orderItem.Amount + orderItem.IncurredCost,
                    Note = request.Note,
                    CreatedAt = DateTime.Now,
                    PaymentStatusId = (int)EnumType.StatusId.Pending
                };

                await _unitOfWork.TransactionRepository.CreateAsync(createItem);
                transactionRes = _mapper.Map<TransactionRes>(createItem);

                string jsonResponse = await this.CreatePaymentURL(createItem.PaymentCode);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Bỏ qua phân biệt chữ hoa/thường
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua giá trị null
                };

                payosRes = JsonSerializer.Deserialize<PayosRes>(jsonResponse, options);

            }
            else
            {
                var createItem = new Transaction()
                {
                    TransactionTypeId = orderItem.TransactionTypeId,
                    OrderId = orderItem.OrderId,
                    PaymentAttempt = 1,
                    PaymentCode = $"{orderItem.OrderId}{DateTime.Now:HHmmssfff}",
                    TotalAmount = orderItem.Amount + orderItem.IncurredCost,
                    Note = request.Note,
                    CreatedAt = DateTime.Now,
                    PaymentStatusId = (int)EnumType.StatusId.Pending
                };

                await _unitOfWork.TransactionRepository.CreateAsync(createItem);
                transactionRes = _mapper.Map<TransactionRes>(createItem);

                string jsonResponse = await this.CreatePaymentURL(createItem.PaymentCode);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Bỏ qua phân biệt chữ hoa/thường
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua giá trị null
                };

                payosRes = JsonSerializer.Deserialize<PayosRes>(jsonResponse, options);

            }
            if (payosRes == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tạo link thanh toán thất bại");
            }

            var responseData = new PaymentRes()
            {
                TransactionRes = transactionRes,
                PayosRes = payosRes,
            };
            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo link thanh toán thành công", responseData);
        }

        public async Task<ServiceResult> Callback(PaymentCallback request)
        {
            var transaction = await _unitOfWork.TransactionRepository.GetByPaymentCode(request.OrderCode);
            if (transaction == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Khong tim thay don hang");
            }
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(transaction.OrderId);
            if (order == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Khong tim thay don hang");
            }

            order.PaymentStatusId = request.Status switch
            {
                "PAID" => (int)EnumType.PaymentStatusId.PAID,
                "PENDING" => (int)EnumType.PaymentStatusId.PENDING,
                "PROCESSING" => (int)EnumType.PaymentStatusId.PROCESSING,
                "CANCELLED" => (int)EnumType.PaymentStatusId.CANCELLED,
            };
            transaction.PaymentStatusId = request.Status switch
            {
                "PAID" => (int)EnumType.PaymentStatusId.PAID,
                "PENDING" => (int)EnumType.PaymentStatusId.PENDING,
                "PROCESSING" => (int)EnumType.PaymentStatusId.PROCESSING,
                "CANCELLED" => (int)EnumType.PaymentStatusId.CANCELLED,
            };

            if (order.PaymentStatusId == (int)EnumType.PaymentStatusId.PAID && 
                   transaction.PaymentStatusId == (int)EnumType.PaymentStatusId.PAID)
            {
                order.PaidAt = DateTime.Now;
                transaction.PaidAt = DateTime.Now;
            }
            else
            {
                order.CancelleddAt = DateTime.Now;
                transaction.CancelleddAt = DateTime.Now;
            }


            await _unitOfWork.OrderRepository.UpdateAsync(order);
            await _unitOfWork.TransactionRepository.UpdateAsync(transaction);

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cap nhat trang thai don hang thanh cong");
        }



        private async Task<string> CreatePaymentURL(string paymentCode)
        {
            var requestUrl = _configuration["PayosSettings:RequestUrl"];
            var clientId = _configuration["PayosSettings:ClientId"];
            var apiKey = _configuration["PayosSettings:ApiKey"];
            var checksumKey = _configuration["PayosSettings:ChecksumKey"];

            var returnUrl = _configuration["PayosSettings:ReturnUrl"];
            var cancelUrl = _configuration["PayosSettings:CancelUrl"];

            var request = await _unitOfWork.TransactionRepository.GetByPaymentCode(paymentCode);

            // Tạo dictionary chứa tham số
            var payload = new Dictionary<string, object>
                             {
                                 { "amount", (int)request.TotalAmount },
                                 { "cancelUrl", cancelUrl },
                                 { "description", request.Note },
                                 { "orderCode", long.Parse(request.PaymentCode) },
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

    }
}
