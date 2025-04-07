using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RentEase.Service.Helper
{

    public class PayOSUtils
    {
        public static string GenerateHmacSha256(string data, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        public static string CreateSortedQueryString(Dictionary<string, object> parameters)
        {
            var sorted = parameters.OrderBy(kv => kv.Key, StringComparer.Ordinal);
            return string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));
        }

    }


}
