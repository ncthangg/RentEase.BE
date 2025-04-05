using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Request;
using RentEase.Service.Service.Main;

namespace RentEase.API.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2,3")]
    public class PayosController : Controller
    {
        private readonly IOrderService _orderService;

        public PayosController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> ReceiveWebhook([FromBody] PayOSWebhookRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderCode))
            {
                return BadRequest(new { message = "Invalid webhook data" });
            }

            // Xử lý các trạng thái từ PayOS
            switch (request.Status.ToUpper())
            {
                case "PAID":
                    await _orderService.UpdatePaymentStatusId(request.OrderCode, (int)EnumType.PaymentStatusId.PAID);
                    break;
                case "CANCELLED":
                    await _orderService.UpdatePaymentStatusId(request.OrderCode, (int)EnumType.PaymentStatusId.CANCELLED);
                    break;
                case "EXPIRED":
                    await _orderService.UpdatePaymentStatusId(request.OrderCode, (int)EnumType.PaymentStatusId.CANCELLED);
                    break;
                default:
                    return BadRequest(new { message = "Unknown status" });
            }

            return Ok(new { message = "Webhook processed successfully" });
        }
    }
}
