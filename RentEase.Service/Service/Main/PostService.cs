using AutoMapper;
using Azure;
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
        Task<ServiceResult> GetAll(bool? status, int page, int pageSize);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetByAptIdAndPostCategoryId(string aptId, int postCategoryId, bool? status);
        Task<ServiceResult> GetByAccountId(string accountId, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(PostReq request);
        Task<ServiceResult> Update(string postId, PostReq request);
        Task<ServiceResult> Active(string id);
        Task<ServiceResult> Deactive(string id);
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
        public async Task<ServiceResult> GetAll(bool? status, int page, int pageSize)
        {

            var items = await _unitOfWork.PostRepository.GetAll(status, page, pageSize);
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
        public async Task<ServiceResult> GetByAccountId(string accountId, bool? status, int page, int pageSize)
        {

            var items = await _unitOfWork.PostRepository.GetByAccountId(accountId, status, page, pageSize);
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

        public async Task<ServiceResult> GetByAptIdAndPostCategoryId(string aptId, int postCategoryId, bool? status)
        {

            var items = await _unitOfWork.PostRepository.GetByAptIdAndPostCategoryId(aptId, postCategoryId, true);
            if (!items.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<PostRes>>(items);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, responseData);
            }
        }
        public async Task<ServiceResult> Create(PostReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            if (string.IsNullOrEmpty(request.AptId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Apt không hợp lệ");
            }

            var items = await _unitOfWork.PostRepository.GetByAccountId(accountId);
            if (items.Count() > 50)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Kho lưu trữ có nhiều hơn 50 bài post, hãy xóa bớt!!");
            }

            var item = await _unitOfWork.PostRepository.GetByAccountIdAndAptIdAsync(accountId, request.AptId);
            if (item != null && item.Status == true)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đã tồn tại Post thuộc về APT này. Hoặc Post còn hiệu lực");
            }

            var aptExist = await _unitOfWork.AptRepository.GetByIdAsync(request.AptId);
            if (aptExist == null || aptExist.AptStatusId != (int)EnumType.AptStatusId.AVAILABLE)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Apt đang trong trạng thái UNAVAILABLE!! hoặc không tồn tại");
            }

            var createItem = new Post()
            {
                PostId = Guid.NewGuid().ToString("N"),
                PostCategoryId = request.PostCategoryId,
                PosterId = accountId,
                AptId = request.AptId,
                Title = request.Title,
                TotalSlot = request.TotalSlot,
                CurrentSlot = request.CurrentSlot,
                PilePrice = request.PilePrice,
                RentPrice = request.RentPrice,
                GenderId = request.GenderId,
                OldId = request.OldId,
                Note = request.Note,
                MoveInDate = request.MoveInDate,
                MoveOutDate = request.MoveOutDate,
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

            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(roleId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("PostId", postId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.PostRepository.GetByIdAsync(postId);

            if (accountId != item.PosterId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (item.Status == true)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bài Post đang ở chế độ PUBLIC!! Không được chỉnh sửa!");
            }

            var updateItem = new Post()
            {
                PostId = item.PostId,
                PostCategoryId = request.PostCategoryId,
                PosterId = item.PostId,
                AptId = item.AptId,
                Title = request.Title,
                TotalSlot = request.TotalSlot,
                CurrentSlot = request.CurrentSlot,
                PilePrice = request.PilePrice,
                RentPrice = request.RentPrice,
                GenderId = request.GenderId,
                OldId = request.OldId,
                Note = request.Note,
                MoveInDate = request.MoveInDate,
                MoveOutDate = request.MoveOutDate,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                DeletedAt = null,
                Status = item.Status
            };
            var result = await _unitOfWork.PostRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Active(string postId)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (roleId != "2")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Chỉ có ROLE: Lessor mới được sử dụng API này");
            }

            var item = await _unitOfWork.PostRepository.GetByIdAsync(postId);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.PosterId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (item.Status == true)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đã PUBLIC");
            }

            var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
            if (account.PublicPostTimes <= 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không đủ lượt sử dụng. Hãy mua Gói!!!");
            }

            if (item.PostCategoryId == (int)EnumType.PostCategoryId.THUENHA)
            {
                var orderType = await _unitOfWork.OrderTypeRepository.GetByPostCategoryId(item.PostCategoryId);

                account.PublicPostTimes -= 1;
                account.UpdatedAt = DateTime.Now;
                await _unitOfWork.AccountRepository.UpdateAsync(account);

                item.Status = true;
                item.StartPublic = DateTime.Now;
                item.EndPublic = DateTime.Now.AddDays(orderType.Days);
                item.UpdatedAt = DateTime.Now;

                var result = await _unitOfWork.PostRepository.UpdateAsync(item);
                if (result > 0)
                {
                    var apt = await _unitOfWork.AptRepository.GetByIdAsync(item.AptId);
                    apt.Status = true;
                    apt.UpdatedAt = DateTime.Now;
                    await _unitOfWork.AptRepository.UpdateAsync(apt);

                    return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Public");
                }

            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Deactive(string postId)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            var item = await _unitOfWork.PostRepository.GetByIdAsync(postId);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.PosterId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (accountId == item.PosterId && roleId == "2")
            {
                var listPost = _unitOfWork.PostRepository.GetByAptId(item.AptId);
                foreach (var post in listPost.Result)
                {
                    post.Status = false;
                    post.UpdatedAt = DateTime.Now;
                    await _unitOfWork.PostRepository.UpdateAsync(post);
                }

                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Private");
            }


            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
