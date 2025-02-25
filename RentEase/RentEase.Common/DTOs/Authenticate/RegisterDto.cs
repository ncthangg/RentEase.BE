using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Common.DTOs.Authenticate
{
    public class RequestRegisterDto
    {
        public string Username;
        public string Password;
        public string ConfirmPassword;
    }

    public class ResponseRegisterDto
    {
        public ResponseAccountDto ResponseAccountDto { get; set; }
        public ResponseAccountVerificationDto ResponseAccountVerificationDto { get; set; }

    }
}
