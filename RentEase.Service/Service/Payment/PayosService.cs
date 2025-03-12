using Microsoft.Extensions.Configuration;
using RentEase.Data;
using RentEase.Service.Helper;
using System.Text;
using System.Text.Json;

namespace RentEase.Service.Service.Payment
{

    public interface IPayosService
    {
        Task<string> CreatePaymentURL(string paymentCode);
        //VnPayRes PaymentExecute(IQueryCollection collections);
    }
    public class PayosService : IPayosService
    {
        private readonly HttpClient _httpClient;
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public PayosService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _unitOfWork ??= new UnitOfWork();
            _configuration = configuration;
        }
        public async Task<string> CreatePaymentURL(string paymentCode)
        {
            var requestUrl = _configuration["PayosSettings:RequestUrl"];
            var clientId = _configuration["PayosSettings:ClientId"];
            var apiKey = _configuration["PayosSettings:ApiKey"];
            var checksumKey = _configuration["PayosSettings:ChecksumKey"];

            var request = await _unitOfWork.TransactionRepository.GetByPaymentCode(paymentCode);

            // Tạo dictionary chứa tham số
            var payload = new Dictionary<string, object>
                             {
                                 { "amount", (int)request.TotalAmount },
                                 { "cancelUrl", "https://your-cancel-url.com" },
                                 { "description", request.Note },
                                 { "orderCode", long.Parse(request.PaymentCode) },
                                 { "returnUrl", "https://your-success-url.com" }
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


        //public VnPayRes PaymentExecute(IQueryCollection collections)
        //{
        //    var vnpay = new VnPayLibrary();
        //    foreach (var (key, value) in collections)
        //    {
        //        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
        //        {
        //            vnpay.AddResponseData(key, value.ToString());
        //        }
        //    }

        //    string vnp_txnRef = vnpay.GetResponseData("vnp_TxnRef");
        //    var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
        //    var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
        //    var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        //    var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

        //    bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnpaySettings:VNP_HASHSECRET"]);
        //    if (!checkSignature)
        //    {
        //        return new VnPayRes
        //        {
        //            Success = false
        //        };
        //    }

        //    return new VnPayRes
        //    {
        //        Success = true,
        //        PaymentMethod = "VnPay",
        //        OrderNote = vnp_OrderInfo,
        //        OrderId = vnp_txnRef,
        //        PaymentId = vnp_txnRef,
        //        TransactionId = vnp_TransactionId.ToString(),
        //        Token = vnp_SecureHash,
        //        VnPayResponseCode = vnp_ResponseCode
        //    };
        //}

    }
}
