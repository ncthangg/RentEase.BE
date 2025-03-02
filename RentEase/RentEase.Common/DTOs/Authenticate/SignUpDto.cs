using RentEase.Common.DTOs.Dto;

namespace RentEase.Common.DTOs.Authenticate
{
    public class RequestRegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required int RoleId { get; set; }
    }

    public class ResponseRegisterDto
    {
        public ResponseAccountDto ResponseAccountDto { get; set; }
        public ResponseWalletDto? ResponseWalletDto { get; set; }
    }
}
