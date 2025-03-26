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
        Task<ServiceResult> GetAll(int? approveStatusId, bool? status, int page, int pageSize);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetByAccountId(string accountId, int? approveStatusId, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(PostReq request);
        Task<ServiceResult> Update(string postId, PostReq request);
        Task<ServiceResult> UpdateApproveStatusId(string postId, int approveStatusId);
        Task<ServiceResult> UpdateStatus(string id);
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
        public async Task<ServiceResult> GetAll(int? approveStatusId, bool? status, int page, int pageSize)
        {

            var items = await _unitOfWork.PostRepository.GetAll(approveStatusId, status, page, pageSize);
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
        public async Task<ServiceResult> GetByAccountId(string accountId, int? approveStatusId, bool? status, int page, int pageSize)
        {

            var items = await _unitOfWork.PostRepository.GetByAccountId(accountId, approveStatusId, status, page, pageSize);
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
                PostCategoryId = request.PostCategoryId,
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
        public async Task<ServiceResult> UpdateApproveStatusId(string postId, int approveStatusId)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("PostId", postId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Post không tồn tại");
            }

            var item = await _unitOfWork.PostRepository.GetByIdAsync(postId);

            if (accountId != item.AccountId || roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (approveStatusId != (int)EnumType.ApproveStatusId.Pending ||
                       approveStatusId != (int)EnumType.ApproveStatusId.Success ||
                            approveStatusId != (int)EnumType.ApproveStatusId.Failed)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "ApproveStatusId không hợp lệ.");
            }

            if (approveStatusId == (int)EnumType.ApproveStatusId.Success)
            {
                item.ApproveStatusId = approveStatusId;
            }
            else
            {
                item.ApproveStatusId = (int)EnumType.ApproveStatusId.Failed;
            }
            item.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.PostRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật Post thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Cập nhật Post thất bại");
        }
        public async Task<ServiceResult> UpdateStatus(string id)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var item = await _unitOfWork.PostRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            item.Status = !item.Status;
            item.UpdatedAt = DateTime.Now;

            await _unitOfWork.PostRepository.UpdateAsync(item);

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
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
