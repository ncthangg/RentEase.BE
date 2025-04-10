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
    public class AccountLikedAptController : Controller
    {
        private readonly IAccountLikedAptService _accountLikedAptService;
        public AccountLikedAptController(IAccountLikedAptService accountLikedAptService)
        {
            _accountLikedAptService = accountLikedAptService;
        }
        [HttpPost("GetByAccountId")]
        public async Task<IActionResult> GetByAccountId([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _accountLikedAptService.GetByAccountId(page, pageSize);

                return Ok(new ApiRes<IEnumerable<AccountLikedAptRes>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Count = result.TotalCount,
                    TotalPages = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    Data = (IEnumerable<AccountLikedAptRes>)result.Data!
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

        [HttpPost("Add-Like")]
        public async Task<IActionResult> AddLike([FromBody] AccountLikedAptReq request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AptId))
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ" });
                }

                await _accountLikedAptService.Create(request.AptId);

                return Ok(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Thêm like thành công"
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

        [HttpPost("Remove-Liked")]
        public async Task<IActionResult> RemoveLiked([FromBody] AccountLikedAptReq request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.AptId))
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ" });
                }

                await _accountLikedAptService.Remove(request.AptId);

                return Ok(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Xóa like thành công"
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

        [HttpPost("Remove-All-Liked")]
        public async Task<IActionResult> RemoveAllLiked()
        {
            try
            {
                await _accountLikedAptService.RemoveAll();

                return Ok(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = "Xóa like thành công"
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
