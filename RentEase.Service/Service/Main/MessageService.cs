using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data.Models;
using RentEase.Data;
using RentEase.Service.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Service.Service.Main
{
    public interface IMessageService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetByConversationId(string conversatiomId);
        Task<ServiceResult> Create(MessageReq request);
        Task<ServiceResult> Delete(int id);

    }
    public class MessageService : BaseService<Message, MessageRes>, IMessageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public MessageService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetByConversationId(string conversationId)
        {
            var items = await _unitOfWork.MessageRepository.GetByConversationIdAsync(conversationId);

            if (items == null || !items.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tìm thấy tin nhắn nào trong cuộc trò chuyện này.");
            }

            var responseData = _mapper.Map<IEnumerable<MessageRes>>(items);
            return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, responseData);
        }

        public async Task<ServiceResult> Create(MessageReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            if (string.IsNullOrEmpty(request.ConversationId) 
                || (await _unitOfWork.ConversationRepository.GetByIdAsync(request.ConversationId) == null))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Sai thông tin ConversationId hoặc ConversationId không tồn tại");
            }

            var createItem = new Message()
            { 
                ConversationId = request.ConversationId,
                SenderId = accountId,
                Content = request.Content,
                SentAt = DateTime.Now,
                IsSeen = false
            };

            var result = await _unitOfWork.MessageRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }



    }
}
