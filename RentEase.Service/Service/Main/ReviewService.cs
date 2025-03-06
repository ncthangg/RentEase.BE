using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IReviewService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status = true);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetAllByAptId(string aptId, int page, int pageSize);
        Task<ServiceResult> Create(ReviewReq request);
        Task<ServiceResult> Update(int id, string comment);
        Task<ServiceResult> Delete(int id);

    }
    public class ReviewService : BaseService<Review, ReviewRes>, IReviewService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public ReviewService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> GetAllByAptId(string aptId, int page, int pageSize)
        {

            var items = await _unitOfWork.ReviewRepository.GetAllForAptAsync(aptId, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ReviewRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(ReviewReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            var createItem = new Review()
            {
                AccountId = accountId,
                AptId = request.AptId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
            };

            var result = await _unitOfWork.ReviewRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(int id, string comment)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);
            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.ReviewRepository.GetByIdAsync(id);

            if (accountId != item.AccountId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Bạn không có quyền hạn.");
            }

            item.Comment = comment;
            item.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.ReviewRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }


    }
}
