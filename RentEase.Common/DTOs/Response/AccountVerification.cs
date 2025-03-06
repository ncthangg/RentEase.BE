namespace RentEase.Common.DTOs.Response
{
    public class AccountVerificationRes
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string VerificationCode { get; set; } = string.Empty;

        public bool? IsUsed { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}


