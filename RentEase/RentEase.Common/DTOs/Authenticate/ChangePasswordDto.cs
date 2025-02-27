namespace RentEase.Common.DTOs.Authenticate
{
    public class RequestChangePasswordDto
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
