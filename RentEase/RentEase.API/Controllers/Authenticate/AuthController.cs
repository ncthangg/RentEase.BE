using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.DTOs.Authenticate;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Request;
using RentEase.Common.DTOs.Response;
using RentEase.Service;
using RentEase.Service.Service.Authenticate;
using System.Net;

namespace RentEase.API.Controllers.Authenticate
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly ServiceWrapper _serviceWrapper;
        private readonly IAuthenticateService _authenticateService;
        private readonly IAccountVerificationService _accountVerificationService;

        public AuthController(
            ServiceWrapper serviceWrapper,
            IAuthenticateService authenticateService,
            IAccountVerificationService accountVerificationService
            )
        {
            _serviceWrapper = serviceWrapper;
            _authenticateService = authenticateService;
            _accountVerificationService = accountVerificationService;
        }

        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(RequestLoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid input data"
                });
            }

            var result = await _authenticateService.SignIn(request);

            if (result.Status != 1)
                return NotFound(new ApiResponse<IEnumerable<ResponseLoginDto>>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message,
                    Data = null
                });

            return Ok(new ApiResponse<ResponseLoginDto>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (ResponseLoginDto)result.Data
            });
        }


        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(RequestRegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid input data"
                });
            }

            var result = await _authenticateService.SignUp(request);

            if (result.Status != 1)
                return NotFound(new ApiResponse<IEnumerable<ResponseRegisterDto>>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message,
                    Data = null
                });

            return Ok(new ApiResponse<ResponseRegisterDto>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (ResponseRegisterDto)result.Data
            });
        }


        //[HttpPost("ChangePassword")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ChangePassword(RequestChangePasswordDto request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new ApiResponse<string>
        //        {
        //            StatusCode = HttpStatusCode.BadRequest,
        //            Message = "Invalid input data"
        //        });
        //    }

        //    var result = await _authenticateService.ChangePassword(request);

        //    if (result.Status != 1)
        //        return NotFound(new ApiResponse<IEnumerable<ResponseRegisterDto>>
        //        {
        //            StatusCode = HttpStatusCode.NotFound,
        //            Message = result.Message,
        //            Data = null
        //        });

        //    return Ok(new ApiResponse<ResponseRegisterDto>
        //    {
        //        StatusCode = HttpStatusCode.OK,
        //        Message = result.Message,
        //        Data = (ResponseRegisterDto)result.Data
        //    });
        //}

        [HttpPost("Verification")]
        public async Task<IActionResult> Verification(RequestVerification request)
        {

            var result = await _accountVerificationService.Verification(request.AccountId, request.VerificationCode);

            if (result.Status != 1)
                return NotFound(new ApiResponse<ResponseAccountDto>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message,
                    Data = null
                });

            return Ok(new ApiResponse<ResponseAccountDto>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (ResponseAccountDto)result.Data
            });
        }

        [HttpGet("GetInfo")]
        public async Task<IActionResult> GetInfo()
        {

            var result = await _authenticateService.GetInfo();

            if (result.Status != 1)
                return NotFound(new ApiResponse<ResponseAccountDto>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message,
                    Data = null
                });

            return Ok(new ApiResponse<ResponseAccountDto>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (ResponseAccountDto)result.Data
            });
        }

    }
}
