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
    public class ContractController : Controller
    {
        private readonly IContractService _ContractService;
        public ContractController(IContractService ContractService)
        {
            _ContractService = ContractService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _ContractService.GetAllAsync(page, pageSize);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseContractDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<IEnumerable<ResponseContractDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Count = result.TotalCount,
                    TotalPages = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    Data = (IEnumerable<ResponseContractDto>)result.Data
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
                var result = await _ContractService.GetByIdAsync(id);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseContractDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseContractDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseContractDto)result.Data
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
        public async Task<IActionResult> Post(RequestContractDto request)
        {
            try
            {
                var result = await _ContractService.Create(request);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseContractDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseContractDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseContractDto)result.Data
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
        public async Task<IActionResult> Put(int id, RequestContractDto request)
        {
            try
            {
                var result = await _ContractService.Update(id, request);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseContractDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseContractDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseContractDto)result.Data
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
                var result = await _ContractService.Delete(id);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseContractDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseContractDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseContractDto)result.Data
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
