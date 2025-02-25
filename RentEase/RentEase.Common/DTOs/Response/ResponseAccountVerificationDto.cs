namespace RentEase.Common.DTOs.Response
{
    public class ResponseAccountVerificationDto
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string VerificationCode { get; set; }

        public bool IsUsed { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}


