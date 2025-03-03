using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;

namespace RentEase.Common.DTOs.Authenticate
{
    public sealed record class RequestLoginDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
    public class ResponseLoginDto
    {
        public ResponseAccountDto ResponseAccountDto { get; set; }
        public string AccessToken { get; set; }
        public ResponseAccountToken ResponseAccountToken { get; set; }
    }
}
