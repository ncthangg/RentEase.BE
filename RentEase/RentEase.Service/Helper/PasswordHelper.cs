using Microsoft.Extensions.Configuration;

namespace RentEase.Service.Helper
{
    public interface IPasswordHelper
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
    public class PasswordHelper : IPasswordHelper
    {
        private readonly IConfiguration _configuration;

        public PasswordHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string HashPassword(string password)
        {
            // Mã hóa mật khẩu với độ phức tạp 12 (có thể thay đổi nếu cần)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // So sánh mật khẩu với giá trị đã hash
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
