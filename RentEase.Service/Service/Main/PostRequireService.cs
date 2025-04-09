using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Ocsp;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;
using static RentEase.Common.Base.EnumType;

namespace RentEase.Service.Service.Main
{

    public interface IPostRequireService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetByAccountId(string accountId, int? approveStatusId, int page, int pageSize);
        Task<ServiceResult> GetByPostId(string postId, int page, int pageSize);
        Task<ServiceResult> Create(PostRequireReq request);
        Task<ServiceResult> UpdateApproveStatusId(string id, PutRequireReq req);
        Task<ServiceResult> Delete(string id);

    }
    public class PostRequireService : BaseService<PostRequire, PostRequireRes>, IPostRequireService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public PostRequireService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetByAccountId(string accountId, int? approveStatusId, int page, int pageSize)
        {

            var items = await _unitOfWork.PostRequireRepository.GetByAccountIdAsync(accountId, approveStatusId, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<PostRequireRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }

        public async Task<ServiceResult> GetByPostId(string postId, int page, int pageSize)
        {
            var items = await _unitOfWork.PostRequireRepository.GetByPostIdAsync(postId, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<PostRequireRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }

        public async Task<ServiceResult> Create(PostRequireReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var postExist = await _unitOfWork.PostRepository.GetByIdAsync(request.PostId);
            if (postExist == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Post không tồn tại!!");
            }

            if (postExist.CurrentSlot >= postExist.TotalSlot)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đã đủ slot");
            }

            var postRequireExist = await _unitOfWork.PostRequireRepository.GetByPostIdAndAccountIdAsync(request.PostId, accountId);
            if (postRequireExist != null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đã có yêu cầu cho bài Post này!!");
            }

            var createItem = new PostRequire()
            {
                Id = Guid.NewGuid().ToString("N"),
                PostId = request.PostId,
                AccountId = accountId,
                ApproveStatusId = (int)EnumType.ApproveStatusId.Pending,
                RequestMessage = request.RequestMessage,
                CreatedAt = DateTime.Now
            };

            var result = await _unitOfWork.PostRequireRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> UpdateApproveStatusId(string id, PutRequireReq req)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(roleId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.PostRequireRepository.GetByIdAsync(id);
            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "PostRequire không tồn tại.");
            }

            if (accountId != item.PostId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (req.ApproveStatusId != (int)EnumType.ApproveStatusId.Pending &&
                     req.ApproveStatusId != (int)EnumType.ApproveStatusId.Success &&
                         req.ApproveStatusId != (int)EnumType.ApproveStatusId.Failed)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "ApproveStatusId không hợp lệ.");
            }

            if (req.ApproveStatusId == (int)EnumType.ApproveStatusId.Success)
            {
                var post = await _unitOfWork.PostRepository.GetByIdAsync(req.PostId);

                if (post.CurrentSlot >= post.TotalSlot)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đã đủ slot");
                }

                post.CurrentSlot += 1;
                post.UpdatedAt = DateTime.Now;
                await _unitOfWork.PostRepository.UpdateAsync(post);
            }

            item.ApproveStatusId = (int)req.ApproveStatusId;
            item.ResponseMessage = req.ResponseMessage;
            item.Note = req.Note;
            item.ResponseAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.PostRequireRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }

    }
}
