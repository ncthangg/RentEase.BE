namespace RentEase.Common.DTOs.Response
{
    public class ResponseAccountToken
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class ResponseToken
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
