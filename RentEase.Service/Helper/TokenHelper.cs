using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentEase.Common.DTOs.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentEase.Service.Helper
{
    public interface ITokenHelper
    {
        Task<TokenRes> GenerateTokens(string accountId, int roleId);
        string GenerateVerificationCode();
        string GenerateAptCode(string categoryName);
        string GetAccountIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor);
        string GetRoleIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor);

    }
    public class TokenHelper : ITokenHelper
    {
        private readonly IConfiguration _configuration;

        public TokenHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //GENERATE JWT TOKEN
        public async Task<TokenRes> GenerateTokens(string accountId, int roleId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim("id", accountId.ToString()),
            new Claim(ClaimTypes.Role, roleId.ToString())
        };

            var accessTokenString = GenerateAccessToken(credentials, claims);
            //var refreshTokenString = GenerateRefreshToken(credentials, claims);
            var refreshTokenString = GenerateRefreshToken();

            return new TokenRes()
            {
                AccessToken = accessTokenString,
                RefreshToken = refreshTokenString
            };
        }

        private string GenerateAccessToken(SigningCredentials credentials, List<Claim> claims)
        {

            var accessToken = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:AccessTokenExpirationMinutes"])),
                signingCredentials: credentials
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            return tokenString;
        }
        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString(); // Tạo refresh token ngẫu nhiên
        }
        //private string GenerateRefreshToken(SigningCredentials credentials, List<Claim> claims)
        //{
        //    var refreshToken = new JwtSecurityToken(
        //        issuer: _configuration["JwtSettings:Issuer"],
        //        audience: _configuration["JwtSettings:Audience"],
        //        claims: claims,
        //        notBefore: DateTime.Now,
        //        expires: DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtSettings:RefreshTokenExpirationDays"])),
        //        signingCredentials: credentials
        //    );
        //    return new JwtSecurityTokenHandler().WriteToken(refreshToken);
        //}


        //GENERATE VERIFICATION CODE
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

        //GENERATE APT CODE
        public string GenerateAptCode(string categoryName)
        {
            string prefix = string.Concat(categoryName.Split(' '))
                                  .ToUpper()
                                  .Substring(0, Math.Min(3, categoryName.Length));

            Random random = new Random();
            int randomNumber = random.Next(100000, 999999);  // 6 số ngẫu nhiên

            return $"{prefix}{randomNumber}";
        }


        //CHECK
        public string GetAccountIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null || !httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                throw new UnauthorizedAccessException("Authorization header is required!");
            }

            string? authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"Invalid authorization header: {authorizationHeader}");
            }

            string jwtToken = authorizationHeader["Bearer ".Length..].Trim();

            if (!ValidateToken(jwtToken, out ClaimsPrincipal principal))
            {
                throw new UnauthorizedAccessException("Token validation failed!");
            }

            var idClaim = principal.Claims.FirstOrDefault(claim => claim.Type == "id");

            return idClaim?.Value ?? throw new UnauthorizedAccessException("User ID claim not found in token!");
        }
        public string GetRoleIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null || !httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                throw new UnauthorizedAccessException("Authorization header is required!");
            }

            string? authorizationHeader = httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException($"Invalid authorization header: {authorizationHeader}");
            }

            string jwtToken = authorizationHeader["Bearer ".Length..].Trim();

            if (!ValidateToken(jwtToken, out ClaimsPrincipal principal))
            {
                throw new UnauthorizedAccessException("Token validation failed!");
            }

            var idClaim = principal.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role);

            return idClaim?.Value ?? throw new UnauthorizedAccessException("Role ID claim not found in token!");
        }


        private bool ValidateToken(string token, out ClaimsPrincipal principal)
        {
            principal = null!;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!); // Ensure UTF-8 encoding

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Use the secret key to verify signature
                    ValidateIssuer = false, // Optional: set to true if you want to validate the issuer
                    ValidateAudience = false, // Optional: set to true if you want to validate the audience
                    ClockSkew = TimeSpan.Zero, // Optional: to account for small time differences
                    RequireSignedTokens = false // To avoid 'kid' requirement issues with symmetric keys
                };

                principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return validatedToken is JwtSecurityToken jwtToken &&
                       jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Token validation failed: " + ex.Message);
                return false;
            }
        }
        private static bool IsTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.Now;
        }
    }
}
