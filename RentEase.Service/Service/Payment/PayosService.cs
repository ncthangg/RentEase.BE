using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Helper;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace RentEase.Service.Service.Payment
{

    public interface IPayosService
    {
        Task<ServiceResult> GetByOrderCode(string code);
        Task<ServiceResult> CheckOut(OrderReq request);
        Task<ServiceResult> Callback(PaymentCallback request);
        Task<ServiceResult> DeleteByOrderCode(string code);
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

        public async Task<ServiceResult> GetByOrderCode(string code)
        {
            var requestUrl = _configuration["PayosSettings:RequestUrl"];
            var clientId = _configuration["PayosSettings:ClientId"];
            var apiKey = _configuration["PayosSettings:ApiKey"];

            var url = $"{requestUrl}/{code}";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var response = await _httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResult(
                    Const.ERROR_EXCEPTION_CODE,
                    $"Lỗi từ PayOS: {response.StatusCode}, Nội dung: {responseString}"
                );
            }

            if (string.IsNullOrEmpty(responseString))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Phản hồi từ PayOS rỗng.");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // Bỏ qua phân biệt chữ hoa/thường
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua giá trị null
            };

            PayosRes? payosRes;
            try
            {
                payosRes = JsonSerializer.Deserialize<PayosRes>(responseString, options);
            }
            catch (JsonException ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, $"Lỗi parse JSON: {ex.Message}");
            }

            if (payosRes == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không thể lấy thông tin thanh toán.");
            }

            var order = await _serviceWrapper.OrderService.GetByOrderCode(code);

            if (order?.Data == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tìm thấy đơn hàng.");
            }

            var responseData = new PaymentRes()
            {
                OrderRes = (OrderRes)order.Data,
                PayosRes = payosRes,
            };

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Lấy thông tin thành công", responseData);
        }
        public async Task<ServiceResult> CheckOut(OrderReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tài khoản không tồn tại");
            }

            if (request.PostId != null)
            {
                var itemList = await _unitOfWork.PostRepository.GetByAptId(request.PostId);
                var publicPostCount = itemList.Count(x => x.Status == true);

                if (publicPostCount >= 3)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Apt này đang có > 3 bài Post đang được Public");
                }
            }

            if (roleId == "3")
            {
                var postExist = await _unitOfWork.PostRepository.GetByAccountId(accountId, true);
                if (postExist != null && postExist.Status == true)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Có Post còn hiệu lực");
                }
            }

            var orderType = await _unitOfWork.OrderTypeRepository.GetByIdAsync(request.OrderTypeId);
            if (orderType == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Gói không khả dụng");
            }

            var orderExisted = await _unitOfWork.OrderRepository.GetByOrderTypeIdAndPostIdAsync(request.OrderTypeId, request.PostId);

            if (orderExisted != null && DateTime.Now < orderExisted.CreatedAt.AddMinutes(10) && orderExisted.PaymentStatusId != (int)EnumType.PaymentStatusId.CANCELLED)
            {
                await this.DeleteByOrderCode(orderExisted.OrderCode);

                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đã xóa link cũ!! Vui lòng tạo link mới");
            }

            if (orderExisted != null && orderExisted.PaymentStatusId == (int)EnumType.PaymentStatusId.PENDING && DateTime.Now > orderExisted.CreatedAt.AddMinutes(10))
            {
                await this.DeleteByOrderCode(orderExisted.OrderCode);
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
                OrderTypeId = orderType.Id,
                OrderCode = orderCode,
                PostId = request.PostId,
                SenderId = accountId,
                TotalAmount = orderType.Amount,
                Note = $"Pay:{orderType.Name.ToUpper()}",
                CreatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.OrderRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var order = await _unitOfWork.OrderRepository.GetByOrderCodeAsync(createItem.OrderCode);
                var orderRes = _mapper.Map<OrderRes>(order);

                string responseString = await this.CreatePaymentURL(createItem.OrderCode);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Bỏ qua phân biệt chữ hoa/thường
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua giá trị null
                };

                PayosRes payosRes = JsonSerializer.Deserialize<PayosRes>(responseString, options);

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
                _ => throw new NotImplementedException(),
            };

            if (order.PaymentStatusId == (int)EnumType.PaymentStatusId.PAID)
            {
                order.PaidAt = DateTime.Now;

                var orderType = await _unitOfWork.OrderTypeRepository.GetByIdAsync(order.OrderTypeId);

                var post = await _unitOfWork.PostRepository.GetByIdAsync(order.PostId);
                post.StartPublic = DateTime.Now;
                post.EndPublic = DateTime.Now.AddDays(orderType.Days);
                post.UpdatedAt = DateTime.Now;
                post.Status = true;

                var apt = await _unitOfWork.AptRepository.GetByIdAsync(post.AptId);
                apt.UpdatedAt = DateTime.Now;
                apt.Status = true;

                await _unitOfWork.PostRepository.UpdateAsync(post);
                await _unitOfWork.AptRepository.UpdateAsync(apt);
            }
            else
            {
                order.CancelledAt = DateTime.Now;
            }

            await _unitOfWork.OrderRepository.UpdateAsync(order);

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cap nhat trang thai don hang thanh cong");
        }
        public async Task<ServiceResult> DeleteByOrderCode(string code)
        {
            var requestUrl = _configuration["PayosSettings:RequestUrl"];
            var clientId = _configuration["PayosSettings:ClientId"];
            var apiKey = _configuration["PayosSettings:ApiKey"];

            var url = $"{requestUrl}/{code}/cancel";

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var payload = new
            {
                reason = "Quá thời hạn",
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ServiceResult(
                    Const.ERROR_EXCEPTION_CODE,
                    $"Lỗi từ PayOS: {response.StatusCode}, Nội dung: {responseString}"
                );
            }

            if (string.IsNullOrEmpty(responseString))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Phản hồi từ PayOS rỗng.");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // Bỏ qua phân biệt chữ hoa/thường
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua giá trị null
            };

            PayosRes? payosRes;
            try
            {
                payosRes = JsonSerializer.Deserialize<PayosRes>(responseString, options);
            }
            catch (JsonException ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, $"Lỗi parse JSON: {ex.Message}");
            }

            if (payosRes == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không thể lấy thông tin thanh toán.");
            }

            var order = await _unitOfWork.OrderRepository.GetByOrderCodeAsync(code);

            if (order == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tìm thấy đơn hàng.");
            }
            order.CancelledAt = DateTime.Now;
            order.PaymentStatusId = (int)EnumType.PaymentStatusId.CANCELLED;

            var updateResult = await _unitOfWork.OrderRepository.UpdateAsync(order);
            if (updateResult <= 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Cập nhật đơn hàng thất bại.");
            }
            var responseData = new PaymentRes()
            {
                OrderRes = _mapper.Map<OrderRes>(order),
                PayosRes = payosRes,
            };

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Lấy thông tin thành công", responseData);
        }

        private async Task<string> CreatePaymentURL(string orderCode)
        {
            var requestUrl = _configuration["PayosSettings:RequestUrl"];

            var clientId = _configuration["PayosSettings:ClientId"];
            var apiKey = _configuration["PayosSettings:ApiKey"];
            var checksumKey = _configuration["PayosSettings:ChecksumKey"];

            var returnUrl = _configuration["PayosSettings:ReturnUrl"];
            var cancelUrl = _configuration["PayosSettings:CancelUrl"];

            var expiredAt = (int)DateTimeOffset.Now.AddMinutes(10).ToUnixTimeSeconds();

            var request = await _unitOfWork.OrderRepository.GetByOrderCodeAsync(orderCode);

            // Tạo dictionary chứa tham số
            var payload = new Dictionary<string, object>
                             {
                                 { "amount", (int)request.TotalAmount },
                                 { "cancelUrl", cancelUrl },
                                 { "description", request.Note },
                                 //{ "expiredAt", (int)DateTimeOffset.Now.AddMinutes(10).ToUnixTimeSeconds()},
                                 { "orderCode", long.Parse(request.OrderCode) },
                                 { "returnUrl", returnUrl }
                             };

            // Sắp xếp tham số và tạo chuỗi signature
            string rawData = PayOSUtils.CreateSortedQueryString(payload);
            string signature = PayOSUtils.GenerateHmacSha256(rawData, checksumKey);

            payload["signature"] = signature;

            bool isValid = IsValidData(rawData, signature, checksumKey);

            var jsonPayload = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            });

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
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
        public static bool IsValidData(string rawData, string transactionSignature, string checksumKey)
        {
            try
            {
                var encoding = Encoding.UTF8;
                using var hmac = new HMACSHA256(encoding.GetBytes(checksumKey));
                var hashBytes = hmac.ComputeHash(encoding.GetBytes(rawData));
                var generatedSignature = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                return generatedSignature == transactionSignature;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in IsValidData: " + ex.Message);
                return false;
            }
        }
        private static string GenerateOrderCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // Sinh số ngẫu nhiên từ 100000 đến 999999
        }
    }
}
