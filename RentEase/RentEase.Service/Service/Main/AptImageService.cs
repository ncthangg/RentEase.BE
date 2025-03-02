using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IAptImageService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Create(RequestAptImageDto request);
        Task<ServiceResult> Update(int id, RequestAptImageDto request);
        Task<ServiceResult> Delete(int id);

    }
    public class AptImageService : BaseService<AptImage, ResponseAptImageDto>, IAptImageService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptImageService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(RequestAptImageDto request)
        {
            if (await EntityExistsAsync("AptId", request.AptId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new AptImage()
            {
                AptId = request.AptId,
                ImageUrl1 = request.ImageUrl1,
                ImageUrl2 = request.ImageUrl2,
                ImageUrl3 = request.ImageUrl3,
                ImageUrl4 = request.ImageUrl4,
                ImageUrl5 = request.ImageUrl5,
                ImageUrl6 = request.ImageUrl6,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.AptImageRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptImageDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestAptImageDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = (AptImage)(await GetByIdAsync(id)).Data;

            var updateItem = new AptImage()
            {
                Id = item.Id,
                AptId = item.AptId,
                ImageUrl1 = request.ImageUrl1,
                ImageUrl2 = request.ImageUrl2,
                ImageUrl3 = request.ImageUrl3,
                ImageUrl4 = request.ImageUrl4,
                ImageUrl5 = request.ImageUrl5,
                ImageUrl6 = request.ImageUrl6,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = null,
                Status = item.Status,
            };

            var result = await _unitOfWork.AptImageRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptImageDto>(updateItem);

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
            var item = (AptImage)(await GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.AptImageRepository.UpdateAsync(item);

            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptImageDto>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
