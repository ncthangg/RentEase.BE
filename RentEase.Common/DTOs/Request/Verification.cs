namespace RentEase.Common.DTOs.Request
{
    public class Verification
    {
        public required string Email { get; set; }
        public required string VerificationCode { get; set; }
    }
}
