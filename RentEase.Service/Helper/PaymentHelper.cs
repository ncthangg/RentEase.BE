using RentEase.Common.DTOs.Response;
using System.Security.Cryptography;
using System.Text;

namespace RentEase.Service.Helper
{

    public class PayOSUtils
    {
        public static string GenerateHmacSha256(string data, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        //public bool ValidateSignature(PaymentCallback request)
        //{
        //    string secretKey = "YOUR_SECRET_KEY"; // Lấy từ PayOS
        //    string rawData = $"{request.OrderCode}{request.Status}{request.Id}";
        //    string computedSignature = ComputeHmacSha256(rawData, secretKey);

        //    return request.Signature == computedSignature;
        //}

        //private string ComputeHmacSha256(string data, string key)
        //{
        //    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        //    byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        //    return Convert.ToHexString(hash).ToLower();
        //}

        public static string CreateSortedQueryString(Dictionary<string, object> parameters)
        {
            return string.Join("&", parameters.OrderBy(k => k.Key).Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }


}
