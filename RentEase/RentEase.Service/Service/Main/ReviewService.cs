using AutoMapper;
using MailKit.Search;
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
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Create(RequestReviewDto request);
        Task<ServiceResult> Update(int id, string comment);
        Task<ServiceResult> Delete(int id);

    }
    public class ReviewService : BaseService<Review, ResponseReviewDto>, IReviewService
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

        public async Task<ServiceResult> Create(RequestReviewDto request)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            if (!int.TryParse(accountId, out int accountIdInt))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "ID tài khoản không hợp lệ");
            }

            var createItem = new Review()
            {
                ReviewerId = accountIdInt,
                AptId = request.AptId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.ReviewRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseReviewDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(int id, string comment)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            if (!int.TryParse(accountId, out int accountIdInt))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "ID tài khoản không hợp lệ");
            }


            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = _mapper.Map<Review>((ResponseReviewDto)(await GetByIdAsync(id)).Data);

            item.Comment = comment;
            item.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.ReviewRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseReviewDto>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Delete(int id)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            var item = (Review)(await GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.ReviewRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseReviewDto>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
