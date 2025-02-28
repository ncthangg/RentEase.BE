using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Service.Service;
using System.Net;

namespace RentEase.API.Controllers.Sub
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _RoleService;
        public RoleController(IRoleService RoleService)
        {
            _RoleService = RoleService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _RoleService.GetAllAsync(page, pageSize);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseRoleDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<IEnumerable<ResponseRoleDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Count = result.TotalCount,
                    TotalPages = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    Data = (IEnumerable<ResponseRoleDto>)result.Data
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
                var result = await _RoleService.GetByIdAsync(id);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseRoleDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseRoleDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseRoleDto)result.Data
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
        public async Task<IActionResult> Post(RequestRoleDto request)
        {
            try
            {
                var result = await _RoleService.Create(request);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseRoleDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseRoleDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseRoleDto)result.Data
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
        public async Task<IActionResult> Put(int id, RequestRoleDto request)
        {
            try
            {
                var result = await _RoleService.Update(id, request);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseRoleDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseRoleDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseRoleDto)result.Data
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
                var result = await _RoleService.DeleteByIdAsync(id);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseRoleDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseRoleDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseRoleDto)result.Data
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
