namespace RentEase.Common.DTOs.Request
{
    public class Verification
    {
        public required int AccountId { get; set; }
        public required string VerificationCode { get; set; }
    }
}
