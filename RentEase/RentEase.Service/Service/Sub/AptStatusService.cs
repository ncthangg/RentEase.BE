using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Sub
{
    public interface IAptStatusService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetAllAsync(bool? status, int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Search(string? statusName, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(RequestAptStatusDto request);
        Task<ServiceResult> Update(int id, RequestAptStatusDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class AptStatusService : BaseService<AptStatus, ResponseAptStatusDto>, IAptStatusService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptStatusService(IHttpContextAccessor httpContextAccessor,IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetAllAsync(bool? status, int page, int pageSize)
        {
            var accountAptStatus = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountAptStatus != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.AptStatusRepository.GetAllAsync(status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAptStatusDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Search(string? statusName, bool? status, int page, int pageSize)
        {
            var accountRole = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountRole != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.AptStatusRepository.GetBySearchAsync(statusName, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAptStatusDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(RequestAptStatusDto request)
        {
            if (await EntityExistsAsync("StatusName", request.StatusName))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new AptStatus()
            {
                StatusName = request.StatusName.ToLower(),
                Description = request.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.AptStatusRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptStatusDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestAptStatusDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = (AptStatus)(await GetByIdAsync(id)).Data;

            var updateItem = new AptStatus()
            {
                Id = id,
                StatusName = request.StatusName.ToLower(),
                Description = request.Description,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = item.DeletedAt,
                Status = item.Status,
            };

            var result = await _unitOfWork.AptStatusRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptStatusDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

    }
}
