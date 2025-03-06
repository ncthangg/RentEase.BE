namespace RentEase.Common.DTOs.Response
{
    public class AccountTokenRes
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class TokenRes
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
