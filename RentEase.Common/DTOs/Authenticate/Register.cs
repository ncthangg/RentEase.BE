using RentEase.Common.DTOs.Dto;

namespace RentEase.Common.DTOs.Authenticate
{
    public class RegisterReq
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required int RoleId { get; set; }
    }

    public class RegisterRes
    {
        public AccountRes AccountRes { get; set; } = new AccountRes();
    }
}
