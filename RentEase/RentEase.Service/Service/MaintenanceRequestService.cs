using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service
{

    public interface IMaintenanceRequestService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestMaintenanceRequestDto request);
        Task<ServiceResult> Update(int id, RequestMaintenanceRequestDto request);
        Task<ServiceResult> Delete(int id);

    }
    public class MaintenanceRequestService : BaseService<MaintenanceRequest, ResponseMaintenanceRequestDto>, IMaintenanceRequestService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public MaintenanceRequestService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> Create(RequestMaintenanceRequestDto request)
        {

            var createItem = new MaintenanceRequest()
            {
                AptId = request.AptId,
                LesseeId = request.LesseeId,
                Description = request.Description,
                Priority = request.Priority,
                AgentId = request.AgentId,
                ApproveStatusId = request.ApproveStatusId,
                ProgressStatusId = request.ProgressStatusId,
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

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestMaintenanceRequestDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new MaintenanceRequest()
            {
                Id = id,
                AptId = request.AptId,
                LesseeId = request.LesseeId,
                Description = request.Description,
                Priority = request.Priority,
                AgentId = request.AgentId,
                ApproveStatusId = request.ApproveStatusId,
                ProgressStatusId = request.ProgressStatusId,
                Note = request.Note,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.Now,
                Status = request.Status,
            };

            var result = await _unitOfWork.MaintenanceRequestRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseMaintenanceRequestDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }

        public async Task<ServiceResult> Delete(int id)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
            }
            var item = (CurrentResident)(await this.GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.CurrentResidentRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseCurrentResidentDto>(item);

                return new ServiceResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
        }
    }
}
