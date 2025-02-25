using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentEase.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RentEase.Service.Helper
{
    public interface ITokenHelper
    {
        string GenerateJWT(Account account, string roleName);
        string GenerateRefreshToken();
        string GenerateVerificationCode();
    }
    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _configuration;

        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJWT(Account account, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim("e", account.Email),
            new Claim("r", roleName)
        };

            var token = new JwtSecurityToken(
                _configuration["JwtSettings:Issuer"],
                _configuration["JwtSettings:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        public string GenerateVerificationCode()
        {
            // Chuỗi chứa các ký tự chữ cái và số
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            // Đối tượng Random
            Random random = new Random();

            // Sinh chuỗi ngẫu nhiên 5 ký tự
            return new string(Enumerable.Repeat(chars, 5)
                                        .Select(s => s[random.Next(s.Length)])
                                        .ToArray());
        }
    }
}
