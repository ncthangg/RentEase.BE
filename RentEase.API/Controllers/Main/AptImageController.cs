using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.Base;
using RentEase.Common.DTOs;
using RentEase.Common.DTOs.Dto;
using RentEase.Service.Service.Main;
using System.Net;

namespace RentEase.API.Controllers.Main
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1,2,3")]
    public class AptImageController : Controller
    {
        private readonly IAptImageService _AptImageService;
        public AptImageController(IAptImageService AptImageService)
        {
            _AptImageService = AptImageService;
        }

        [HttpGet("GetByAptId")]
        public async Task<IActionResult> GetByAptId([FromQuery] string aptId)
        {
            try
            {
                var result = await _AptImageService.GetByAptId(aptId);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<AptImageRes>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (AptImageRes)result.Data!
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

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] PostListAptImageReq request)
        {
            try
            {
                var result = await _AptImageService.Create(request.AptId, request.Files);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<IEnumerable<string>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data= (IEnumerable<string>)result.Data! 
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

        [HttpPut("UpdateSingleImage")]
        public async Task<IActionResult> UpdateSingleImage([FromQuery] int id, [FromQuery] PostSingleAptImageReq request)
        {
            try
            {
                var result = await _AptImageService.UpdateSingleImage(id, request);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message
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

        [HttpDelete]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var result = await _AptImageService.Delete(imageId);
            if (result.Status == Const.SUCCESS_ACTION_CODE)
            {
                return Ok(new ApiRes<string>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message
                });
            }
            return BadRequest(new ApiRes<string>
            {
                StatusCode = HttpStatusCode.BadRequest,
                Message = result.Message
            }); ;
        }
    }
}
