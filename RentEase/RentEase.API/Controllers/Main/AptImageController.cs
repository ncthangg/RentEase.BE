using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Service.Service;
using System.Net;

namespace RentEase.API.Controllers.Main
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class AptImageController : Controller
    {
        private readonly IAptImageService _AptImageService;
        public AptImageController(IAptImageService AptImageService)
        {
            _AptImageService = AptImageService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _AptImageService.GetAllAsync(page, pageSize);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseAptImageDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<IEnumerable<ResponseAptImageDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Count = result.TotalCount,
                    TotalPages = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    Data = (IEnumerable<ResponseAptImageDto>)result.Data
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
                var result = await _AptImageService.GetByIdAsync(id);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseAptImageDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseAptImageDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseAptImageDto)result.Data
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
        public async Task<IActionResult> Post(RequestAptImageDto request)
        {
            try
            {
                var result = await _AptImageService.Create(request);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseAptImageDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseAptImageDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseAptImageDto)result.Data
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
        public async Task<IActionResult> Put(int id, RequestAptImageDto request)
        {
            try
            {
                var result = await _AptImageService.Update(id, request);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseAptImageDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseAptImageDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseAptImageDto)result.Data
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
                var result = await _AptImageService.Delete(id);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseAptImageDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseAptImageDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseAptImageDto)result.Data
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
