using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.DTOs;
using RentEase.Common.DTOs.Dto;
using RentEase.Service.Service.Main;
using System.Net;

namespace RentEase.API.Controllers.Main
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2,3")]
    public class AptUtilityController : Controller
    {
        private readonly IAptUtilityService _aptUtilityService;
        public AptUtilityController(IAptUtilityService aptUtilityService)
        {
            _aptUtilityService = aptUtilityService;
        }
        [HttpPost("GetByAptId")]
        public async Task<IActionResult> GetByAptId([FromQuery] string aptId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(aptId))
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ" });
                }

                var result = await _aptUtilityService.GetByAptId(aptId, page, pageSize);

                return Ok(new ApiRes<AptUtilityRes>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (AptUtilityRes)result.Data!
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


        [HttpPost("Add-Utilities")]
        public async Task<IActionResult> AddUtilities([FromBody] AptUtilityReq request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AptId) || request.Utilities == null || !request.Utilities.Any())
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ" });
                }

                foreach (var utility in request.Utilities)
                {
                    // Lưu vào database hoặc gọi service xử lý
                    await _aptUtilityService.Create(request.AptId, utility.UtilityId, utility.Note);
                }

                return Ok(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Thêm tiện ích thành công"
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
