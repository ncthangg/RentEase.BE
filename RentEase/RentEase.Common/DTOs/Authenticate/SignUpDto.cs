using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;

namespace RentEase.Common.DTOs.Authenticate
{
    public class RequestRegisterDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public RequestAccountDto Account { get; set; }
    }

    public class ResponseRegisterDto
    {
        public ResponseAccountDto ResponseAccountDto { get; set; }
        public ResponseAccountVerification ResponseAccountVerificationDto { get; set; }

    }
}
