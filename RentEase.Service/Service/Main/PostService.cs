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
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetByAccountId(string accountId, int? statusId, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(PostReq request);
        Task<ServiceResult> Update(string postId, PostReq request);
        Task<ServiceResult> DeleteSoft(string id);
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
        public async Task<ServiceResult> GetByAccountId(string accountId, int? statusId, bool? status, int page, int pageSize)
        {

            var items = await _unitOfWork.PostRepository.GetByAccountId(accountId, statusId, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<PostRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(PostReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
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
                ApproveStatusId = (int)EnumType.ApproveStatusId.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = false
            };

            var result = await _unitOfWork.PostRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(string postId, PostReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("PostId", postId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.PostRepository.GetByIdAsync(postId);

            if (accountId != item.PostId || roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (request.ApproveStatusId != (int)EnumType.ApproveStatusId.Pending &&
                     request.ApproveStatusId != (int)EnumType.ApproveStatusId.Success &&
                         request.ApproveStatusId != (int)EnumType.ApproveStatusId.Failed)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "ApproveStatusId không hợp lệ.");
            }

            var result = await _unitOfWork.PostRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> DeleteSoft(string id)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            var item = await _unitOfWork.PostRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.AccountId || roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            item.DeletedAt = DateTime.Now;
            item.Status = false;

            var result = await _unitOfWork.PostRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Xóa mềm thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
