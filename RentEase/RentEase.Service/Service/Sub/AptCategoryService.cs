
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
                return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAptCategoryDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
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
                return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAptCategoryDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }

        public async Task<ServiceResult> Create(RequestAptCategoryDto request)
        {
            if (await EntityExistsAsync("CategoryName", request.CategoryName))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
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

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestAptCategoryDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new AptCategory()
            {
                Id = id,
                CategoryName = request.CategoryName.ToLower(),
                Description = request.Description,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = request.DeletedAt,
                Status = request.Status,
            };

            var result = await _unitOfWork.AptCategoryRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptCategoryDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);

        }


    }
}
