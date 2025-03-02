using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Sub
{
    public interface IUtilityService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetAllAsync(bool? status, int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Search(string? utilityName, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(RequestUtilityDto request);
        Task<ServiceResult> Update(int id, RequestUtilityDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);
    }
    public class UtilityService : BaseService<Utility, ResponseUtilityDto>, IUtilityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public UtilityService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetAllAsync(bool? status, int page, int pageSize)
        {
            var accountUtility = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountUtility != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.UtilityRepository.GetAllAsync(status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseUtilityDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Search(string? utilityName, bool? status, int page, int pageSize)
        {
            var accountRole = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountRole != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.UtilityRepository.GetBySearchAsync(utilityName, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseUtilityDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }

        public async Task<ServiceResult> Create(RequestUtilityDto request)
        {
            if (await EntityExistsAsync("UtilityName", request.UtilityName))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            var createItem = new Utility()
            {
                UtilityName = request.UtilityName.ToLower(),
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.UtilityRepository.CreateAsync(createItem);

            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseUtilityDto>(createItem);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestUtilityDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new Utility()
            {
                Id = id,
                UtilityName = request.UtilityName.ToLower(),
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = request.DeletedAt,
                Status = request.Status,
            };

            var result = await _unitOfWork.UtilityRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseUtilityDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);

        }
    }
}
