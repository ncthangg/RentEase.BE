using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;
using static RentEase.Common.Base.EnumType;

namespace RentEase.Service.Service.Main
{

    public interface ICurrentResidentService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Create(RequestCurrentResidentDto request);
        Task<ServiceResult> Update(int id, int? liveStatus);
        Task<ServiceResult> Delete(int id);

    }
    public class CurrentResidentService : BaseService<CurrentResident, ResponseCurrentResidentDto>, ICurrentResidentService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public CurrentResidentService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(RequestCurrentResidentDto request)
        {
            var createItem = new CurrentResident()
            {
                AptId = request.AptId,
                AccountId = request.AccountId,
                MoveInDate = request.MoveInDate,
                MoveOutDate = request.MoveOutDate,
                StatusId = (int)EnumType.LiveStatus.Active,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.CurrentResidentRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseCurrentResidentDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(int id, int? liveStatus)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            var item = (CurrentResident)(await GetByIdAsync(id)).Data;

            if (liveStatus != (int)EnumType.LiveStatus.Active &&
                    liveStatus != (int)EnumType.LiveStatus.MoveOut)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "LiveStatus không hợp lệ.");
            }

            var updateItem = new CurrentResident()
            {
                Id = item.Id,
                AptId = item.AptId,
                AccountId = item.AccountId,
                MoveInDate = item.MoveInDate,
                MoveOutDate = item.MoveOutDate,
                StatusId = (int)liveStatus,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                Status = item.Status,
            };

            var result = await _unitOfWork.CurrentResidentRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseCurrentResidentDto>(updateItem);

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
            var item = (CurrentResident)(await GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.CurrentResidentRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseCurrentResidentDto>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
