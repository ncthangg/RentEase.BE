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
    public class AptController : ControllerBase
    {
        private readonly IAptService _aptService;
        public AptController(IAptService aptService)
        {
            _aptService = aptService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _aptService.GetAll(page, pageSize, status);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<IEnumerable<AptRes>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Count = result.TotalCount,
                    TotalPages = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    Data = (IEnumerable<AptRes>)result.Data!
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

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery] string aptId)
        {
            try
            {
                var result = await _aptService.GetById(aptId);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<AptRes>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<AptRes>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (AptRes)result.Data!
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

        [HttpGet("GetByAccountId")]
        public async Task<IActionResult> GetByAccountId([FromQuery] string accountId, [FromQuery] int? statusId, [FromQuery] bool? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _aptService.GetByAccountId(accountId, statusId, status, page, pageSize);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<IEnumerable<AptRes>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Count = result.TotalCount,
                    TotalPages = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    Data = (IEnumerable<AptRes>)result.Data!
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
        public async Task<IActionResult> Post([FromBody] AptReq request)
        {
            try
            {
                var result = await _aptService.Create(request);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<AptRes>
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

        [HttpPut]
        public async Task<IActionResult> Put(string aptId, [FromBody] AptReq request)
        {
            try
            {
                var result = await _aptService.Update(aptId, request);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<AptRes>
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

        [HttpPost("Update-Status")]
        public async Task<IActionResult> UpdateStatus(string aptId)
        {
            try
            {
                var result = await _aptService.UpdateStatus(aptId);
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

        [HttpPatch]
        public async Task<IActionResult> DeleteSoft(string aptId)
        {
            try
            {
                var result = await _aptService.DeleteSoft(aptId);
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
        public async Task<IActionResult> Delete(string aptId)
        {
            try
            {
                var result = await _aptService.Delete(aptId);
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
    }
}
