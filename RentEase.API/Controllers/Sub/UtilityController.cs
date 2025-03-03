using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Service.Service.Sub;
using System.Net;

namespace RentEase.API.Controllers.Sub
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class UtilityController : ControllerBase
    {
        private readonly IUtilityService _utilityService;
        public UtilityController(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool status = true, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _utilityService.GetAllAsync(status, page, pageSize);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiResponse<IEnumerable<ResponseUtilityDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Count = result.TotalCount,
                    TotalPages = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    Data = (IEnumerable<ResponseUtilityDto>)result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = await _utilityService.GetByIdAsync(id);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiResponse<ResponseUtilityDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseUtilityDto)result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name, [FromQuery] bool status = true, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(new { message = "Name is required" });
                }

                var result = await _utilityService.Search(name, status, page, pageSize);

                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiResponse<IEnumerable<ResponseUtilityDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (IEnumerable<ResponseUtilityDto>)result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(RequestUtilityDto request)
        {
            try
            {
                var result = await _utilityService.Create(request);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiResponse<ResponseUtilityDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseUtilityDto)result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, RequestUtilityDto request)
        {
            try
            {
                var result = await _utilityService.Update(id, request);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiResponse<ResponseUtilityDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseUtilityDto)result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _utilityService.DeleteByIdAsync(id);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiResponse<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiResponse<ResponseUtilityDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseUtilityDto)result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Message = $"Lỗi hệ thống: {ex.Message}"
                });
            }
        }
    }
}
