namespace RentEase.Common.DTOs.Response
{
    public class ResponseAccountTokenDto
    {
        public int TokenId { get; set; }

        public int? AccountId { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? RefreshTokenExpires { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}
