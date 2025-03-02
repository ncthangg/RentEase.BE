using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IMaintenanceRequestService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Create(RequestMaintenanceRequestDto request);
        Task<ServiceResult> Update(int id, RequestMaintenanceRequestDto request, int? approveStatus, int? progressStatus);
        Task<ServiceResult> Delete(int id);

    }
    public class MaintenanceRequestService : BaseService<MaintenanceRequest, ResponseMaintenanceRequestDto>, IMaintenanceRequestService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public MaintenanceRequestService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> Create(RequestMaintenanceRequestDto request)
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

            var createItem = new MaintenanceRequest()
            {
                AptId = request.AptId,
                LesseeId = accountIdInt,
                Description = request.Description,
                Priority = request.Priority,
                AgentId = 0,
                ApproveStatusId = (int)EnumType.ApproveStatus.Pending,
                ProgressStatusId = (int)EnumType.ProgressStatus.NotYet,
                Note = request.Note,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.MaintenanceRequestRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseMaintenanceRequestDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestMaintenanceRequestDto request, int? approveStatus, int? progressStatus)
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

            var item = _mapper.Map<MaintenanceRequest>((ResponseMaintenanceRequestDto)(await GetByIdAsync(id)).Data);

            if (approveStatus != (int)EnumType.ApproveStatus.Pending &&
                        approveStatus != (int)EnumType.ApproveStatus.Approved &&
                             approveStatus != (int)EnumType.ApproveStatus.Rejected)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "ApproveStatus không hợp lệ.");
            }

            if (progressStatus != (int)EnumType.ProgressStatus.NotYet &&
                    progressStatus != (int)EnumType.ProgressStatus.InProgress &&
                        progressStatus != (int)EnumType.ProgressStatus.Done)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "ProgressStatus không hợp lệ.");
            }

            var updateItem = new MaintenanceRequest()
            {
                Id = item.Id,
                AptId = item.AptId,
                LesseeId = item.LesseeId,
                Description = request.Description,
                Priority = request.Priority,
                AgentId = item.AgentId,
                ApproveStatusId = (int)approveStatus,
                ProgressStatusId = (int)progressStatus,
                Note = request.Note,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                Status = item.Status,
            };

            var result = await _unitOfWork.MaintenanceRequestRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseMaintenanceRequestDto>(updateItem);

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
            var item = (MaintenanceRequest)(await GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.MaintenanceRequestRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseMaintenanceRequestDto>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
