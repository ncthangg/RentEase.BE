namespace RentEase.Common.DTOs.Response
{
    public class ResponseAccountToken
    {
        public int TokenId { get; set; }

        public int? AccountId { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? RefreshTokenExpires { get; set; }

        public DateTime? LastLogin { get; set; }
    }

    public class ResponseToken
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
