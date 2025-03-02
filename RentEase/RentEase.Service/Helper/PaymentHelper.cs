using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace RentEase.Service.Helper
{
    public interface IPaymentHelper
    {
        string GetPaymentUrl(string orderId, decimal amount);
    }

    public class PaymentHelper : IPaymentHelper
    {
        private readonly IConfiguration _configuration;

        public PaymentHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        private const string VNPAY_RETURN_URL = "myapp://vnpay_return";
        public string GetPaymentUrl(string orderId, decimal amount)
        {
            string vnpVersion = "2.1.0";
            string vnpCommand = "pay";
            string vnpTmnCode = _configuration["VNPaySettings:VNP_TmnCode"];
            string vnpTxnRef = orderId;
            string vnpOrderInfo = "Thanh toan XXX " + orderId;
            string vnpOrderType = "other";
            string vnpLocale = "vn";
            string vnpIpAddr = "127.0.0.1";

            long vnpAmount = (long)(amount * 100); // VNPay yêu cầu nhân 100
            string vnpCreateDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            var vnpParams = new SortedDictionary<string, string>
        {
            { "vnp_Version", vnpVersion },
            { "vnp_Command", vnpCommand },
            { "vnp_TmnCode", vnpTmnCode },
            { "vnp_Amount", vnpAmount.ToString() },
            { "vnp_CurrCode", "VND" },
            { "vnp_TxnRef", vnpTxnRef },
            { "vnp_OrderInfo", vnpOrderInfo },
            { "vnp_OrderType", vnpOrderType },
            { "vnp_Locale", vnpLocale },
            { "vnp_ReturnUrl", VNPAY_RETURN_URL },
            { "vnp_IpAddr", vnpIpAddr },
            { "vnp_CreateDate", vnpCreateDate }
        };

            string queryString = string.Join("&", vnpParams.Select(kvp => kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value)));
            string secureHash = HmacSHA512(_configuration["VNPaySettings:VNP_HashSecret"], queryString);

            return $"{_configuration["VNPaySettings:VNP_Url"]}?{queryString}&vnp_SecureHash={secureHash}";
        }

        private static string HmacSHA512(string key, string data)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
