using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Common.DTOs.Authenticate
{
    public sealed record class RequestLoginDto
    {
        public string Username;
        public string Password;
    }
    public class ResponseLoginDto
    {
        public ResponseAccountDto ResponseAccountDto { get; set; }
        public ResponseAccountTokenDto ResponseAccountTokenDto { get; set; }
        public string Token { get; set; }  // JWT Token
    }
}
