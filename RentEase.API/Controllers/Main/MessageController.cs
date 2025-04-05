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
    public class MessageController : Controller
    {
        private readonly IMessageService _MessageService;
        public MessageController(IMessageService MessageService)
        {
            _MessageService = MessageService;
        }

        //[HttpGet("GetAll")]
        //public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    try
        //    {
        //        var result = await _MessageService.GetAll(page, pageSize, null);
        //        if (result.Status < 0 && result.Data == null)
        //        {
        //            return NotFound(new ApiRes<string>
        //            {
        //                StatusCode = HttpStatusCode.NotFound,
        //                Message = result.Message
        //            });
        //        }
        //        return Ok(new ApiRes<IEnumerable<MessageRes>>
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Message = result.Message,
        //            Count = result.TotalCount,
        //            TotalPages = result.TotalPage,
        //            CurrentPage = result.CurrentPage,
        //            Data = (IEnumerable<MessageRes>)result.Data!
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiRes<string>
        //        {
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            Message = $"Lỗi hệ thống: {ex.Message}"
        //        });
        //    }
        //}

        //[HttpGet("GetById")]
        //public async Task<IActionResult> GetById([FromQuery] int id)
        //{
        //    try
        //    {
        //        var result = await _MessageService.GetById(id);
        //        if (result.Status < 0 && result.Data == null)
        //        {
        //            return NotFound(new ApiRes<string>
        //            {
        //                StatusCode = HttpStatusCode.NotFound,
        //                Message = result.Message
        //            });
        //        }
        //        return Ok(new ApiRes<MessageRes>
        //        {
        //            StatusCode = HttpStatusCode.OK,
        //            Message = result.Message,
        //            Data = (MessageRes)result.Data!
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiRes<string>
        //        {
        //            StatusCode = HttpStatusCode.InternalServerError,
        //            Message = $"Lỗi hệ thống: {ex.Message}"
        //        });
        //    }
        //}

        [HttpGet("GetByConversationId")]
        public async Task<IActionResult> GetByConversationId([FromQuery] string conversationId)
        {
            try
            {
                var result = await _MessageService.GetByConversationId(conversationId);
                if (result.Status < 0 && result.Data == null)
                {
                    return NotFound(new ApiRes<string>
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Message = result.Message
                    });
                }
                return Ok(new ApiRes<IEnumerable<MessageRes>>
                {
                    StatusCode = HttpStatusCode.OK,
                    Message = result.Message,
                    Data = (IEnumerable<MessageRes>)result.Data!
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
        public async Task<IActionResult> Post([FromBody] MessageReq request)
        {
            try
            {
                var result = await _MessageService.Create(request);
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _MessageService.Delete(id);
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
