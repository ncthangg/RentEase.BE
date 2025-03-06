using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{

    public interface IPostService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status = true);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetAllOwn(int? statusId, int page, int pageSize, bool? status = true);
        Task<ServiceResult> Create(PostReq request);
        Task<ServiceResult> Update(string postId, PostReq request);
        Task<ServiceResult> Delete(string id);
    }
    public class PostService : BaseService<Post, PostRes>, IPostService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public PostService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetAllOwn(int? statusId, int page, int pageSize, bool? status = true)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            var items = await _unitOfWork.PostRepository.GetAllOwn(accountId, statusId, page, pageSize, status);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<PostRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(PostReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            var createItem = new Post()
            {
                PostId = Guid.NewGuid().ToString("N"),
                AccountId = accountId,
                AptId = request.AptId,
                Title = request.Title,
                TotalSlot = request.TotalSlot,
                CurrentSlot = request.CurrentSlot,
                GenderId = request.GenderId,
                OldId = request.OldId,
                Note = request.Note,
                MoveInDate = request.MoveInDate,
                MoveOutDate = request.MoveOutDate,
                StatusId = (int)EnumType.StatusId.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true
            };

            var result = await _unitOfWork.PostRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(string postId, PostReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("PostId", postId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.PostRepository.GetByIdAsync(postId);

            if (accountId != item.AccountId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Bạn không có quyền hạn.");
            }

            if (request.StatusId != (int)EnumType.StatusId.Pending &&
                     request.StatusId != (int)EnumType.StatusId.Success &&
                         request.StatusId != (int)EnumType.StatusId.Failed)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "StatusId không hợp lệ.");
            }

            var result = await _unitOfWork.PostRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
