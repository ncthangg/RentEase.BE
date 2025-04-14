using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.DTOs;
using RentEase.Common.DTOs.Authenticate;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Request;
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
        public async Task<IActionResult> SignIn(LoginReq request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid input data"
                });
            }

            var result = await _authenticateService.SignIn(request);

            if (result.Status < 0)
                return NotFound(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message
                });

            return Ok(new ApiRes<LoginRes>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (LoginRes)result.Data!
            });
        }

        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(RegisterReq request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid input data"
                });
            }

            var result = await _authenticateService.SignUp(request);

            if (result.Status < 0)
                return NotFound(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message
                });

            return Ok(new ApiRes<RegisterRes>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (RegisterRes)result.Data!
            });
        }

        [HttpPost("ChangePassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromQuery] string accountId, ChangePasswordReq request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid input data"
                });
            }

            var result = await _authenticateService.ChangePassword(accountId, request);

            if (result.Status < 0)
                return NotFound(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message
                });

            return Ok(new ApiRes<RegisterRes>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (RegisterRes)result.Data!
            });
        }

        [HttpPost("Verification")]
        [AllowAnonymous]
        public async Task<IActionResult> Verification(Verification request)
        {

            var result = await _accountVerificationService.Verification(request.Email, request.VerificationCode);

            if (result.Status < 0)
                return NotFound(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message
                });

            return Ok(new ApiRes<RegisterRes>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (RegisterRes)result.Data!
            });
        }

        [HttpGet("GetInfo")]
        [Authorize(Roles = "1,2,3")]
        public async Task<IActionResult> GetInfo()
        {
            var result = await _authenticateService.GetInfo();

            if (result.Status < 0)
                return NotFound(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = result.Message
                });

            return Ok(new ApiRes<AccountRes>
            {
                StatusCode = HttpStatusCode.OK,
                Message = result.Message,
                Data = (AccountRes)result.Data!
            });
        }


    }
}
