using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.DTOs;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Service;
using RentEase.Service.Service.Payment;
using System.Net;

namespace RentEase.API.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2,3")]
    public class PaymentController : Controller
    {
        private readonly ServiceWrapper _serviceWrapper;
        private readonly IPayosService _payosService;

        public PaymentController(ServiceWrapper serviceWrapper, IPayosService payosService)
        {
            _serviceWrapper = serviceWrapper;
            _payosService = payosService;
        }
        [HttpGet("GetByOrderCode")]
        public async Task<IActionResult> GetByOrderCode([FromQuery] string code)
        {
            try
            {
                var result = await _payosService.GetByOrderCode(code);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<PaymentRes>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (PaymentRes)result.Data!
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpPost("Create-Payment-Link")]
        public async Task<IActionResult> Post([FromBody] OrderReq request)
        {
            try
            {
                var result = await _payosService.CheckOut(request);
                if (result.Status < 0)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<PaymentRes>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (PaymentRes)result.Data!
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpGet("Payment-Callback")]
        public async Task<IActionResult> PaymentCallback([FromQuery] PaymentCallback request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Id) || string.IsNullOrEmpty(request.OrderCode))
                {
                    return BadRequest(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                var result = await _payosService.Callback(request);
                if (result.Status < 0)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<PaymentRes>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (PaymentRes)result.Data!
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpPost("Delete-Payment-Link")]
        public async Task<IActionResult> Delete([FromQuery] string code)
        {
            try
            {
                var result = await _payosService.DeleteByOrderCode(code);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<PaymentRes>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (PaymentRes)result.Data!
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }
    }
}
