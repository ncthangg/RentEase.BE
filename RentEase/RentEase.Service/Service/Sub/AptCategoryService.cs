
using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Sub
{
    public interface IAptCategoryService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetAllAsync(bool? status, int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Search(string? categoryName, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(RequestAptCategoryDto request);
        Task<ServiceResult> Update(int id, RequestAptCategoryDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class AptCategoryService : BaseService<AptCategory, ResponseAptCategoryDto>, IAptCategoryService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptCategoryService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetAllAsync(bool? status, int page, int pageSize)
        {
            var accountAptCategory = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountAptCategory != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.AptCategoryRepository.GetAllAsync(status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAptCategoryDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Search(string? categoryName, bool? status, int page, int pageSize)
        {
            var accountAptCategory = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountAptCategory != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.AptCategoryRepository.GetBySearchAsync(categoryName, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAptCategoryDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }

        public async Task<ServiceResult> Create(RequestAptCategoryDto request)
        {
            if (await EntityExistsAsync("CategoryName", request.CategoryName))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new AptCategory()
            {
                CategoryName = request.CategoryName.ToLower(),
                Description = request.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.AptCategoryRepository.CreateAsync(createItem);

            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptCategoryDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestAptCategoryDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            var item = (AptCategory)(await GetByIdAsync(id)).Data;

            var updateItem = new AptCategory()
            {
                Id = id,
                CategoryName = request.CategoryName.ToLower(),
                Description = request.Description,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = item.DeletedAt,
                Status = item.Status,
            };

            var result = await _unitOfWork.AptCategoryRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptCategoryDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);

        }


    }
}
