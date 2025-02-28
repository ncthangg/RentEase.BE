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
    public class TransactionTypeController : ControllerBase
    {
        private readonly ITransactionTypeService _TransactionTypeService;
        public TransactionTypeController(ITransactionTypeService TransactionTypeService)
        {
            _TransactionTypeService = TransactionTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _TransactionTypeService.GetAllAsync(page, pageSize);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseTransactionTypeDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<IEnumerable<ResponseTransactionTypeDto>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Count = result.TotalCount,
                    TotalPages = result.TotalPage,
                    CurrentPage = result.CurrentPage,
                    Data = (IEnumerable<ResponseTransactionTypeDto>)result.Data
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
                var result = await _TransactionTypeService.GetByIdAsync(id);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseTransactionTypeDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseTransactionTypeDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseTransactionTypeDto)result.Data
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
        public async Task<IActionResult> Post(RequestTransactionTypeDto request)
        {
            try
            {
                var result = await _TransactionTypeService.Create(request);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseTransactionTypeDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseTransactionTypeDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseTransactionTypeDto)result.Data
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
        public async Task<IActionResult> Put(int id, RequestTransactionTypeDto request)
        {
            try
            {
                var result = await _TransactionTypeService.Update(id, request);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseTransactionTypeDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseTransactionTypeDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseTransactionTypeDto)result.Data
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
                var result = await _TransactionTypeService.DeleteByIdAsync(id);
                if (result.Data == null)
                {
                    return Ok(new ApiResponse<ResponseTransactionTypeDto>
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "No Data",
                        Data = null
                    });
                }
                return Ok(new ApiResponse<ResponseTransactionTypeDto>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (ResponseTransactionTypeDto)result.Data
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
