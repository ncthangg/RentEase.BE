using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.DTOs;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Service;
using System.Net;

namespace RentEase.API.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2,3,4")]
    public class PaymentController : Controller
    {
        private readonly ServiceWrapper _serviceWrapper;

        public PaymentController(ServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }
        [HttpPost("create-payment-link")]
        public async Task<IActionResult> Post([FromBody] TransactionReq request)
        {
            try
            {
                var result = await _serviceWrapper.TransactionService.CheckOut(request);
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
    }
}
