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

        public static string CreateSortedQueryString(Dictionary<string, object> parameters)
        {
            return string.Join("&", parameters.OrderBy(k => k.Key).Select(kv => $"{kv.Key}={kv.Value}"));
        }
    }


}
