using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;

namespace RentEase.Common.DTOs.Authenticate
{
    public sealed record class LoginReq
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
    public class LoginRes
    {
        public AccountRes AccountRes { get; set; } = new AccountRes();
        public string AccessToken { get; set; } = string.Empty;
        public AccountTokenRes AccountTokenRes { get; set; } = new AccountTokenRes();
    }
}
