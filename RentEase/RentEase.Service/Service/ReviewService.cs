using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service
{
    public interface IReviewService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestReviewDto request);
        Task<ServiceResult> Update(int id, RequestReviewDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class ReviewService : BaseService<Review, ResponseReviewDto>, IReviewService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public ReviewService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public Task<ServiceResult> Search(string name)
        {
            throw new NotImplementedException();
        }
        //public async Task<ServiceResult> Search(string name)
        //{
        //    var Reviews = await _unitOfWork.ReviewRepository.GetByFullNameAsync(name);
        //    if (!Reviews.Any())
        //    {
        //        return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, null);
        //    }
        //    else
        //    {
        //        var response = _mapper.Map<IEnumerable<ResponseReviewDto>>(Reviews);
        //        return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, response, response.Count());
        //    }
        //}

        public async Task<ServiceResult> Create(RequestReviewDto request)
        {
            var createItem = new Review()
            {
                ReviewerId = request.ReviewerId,
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

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }



        public async Task<ServiceResult> Update(int id, RequestReviewDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new Review()
            {
                ReviewerId = request.ReviewerId,
                AptId = request.AptId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.Now,
                Status = request.Status,
            };

            var result = await _unitOfWork.ReviewRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseReviewDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }
    }
}
