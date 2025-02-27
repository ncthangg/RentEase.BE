using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service
{

    public interface ICurrentResidentService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestCurrentResidentDto request);
        Task<ServiceResult> Update(int id, RequestCurrentResidentDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

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
                StatusId = request.StatusId,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.CurrentResidentRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseCurrentResidentDto>(createItem);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestCurrentResidentDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new CurrentResident()
            {
                AptId = request.AptId,
                AccountId = request.AccountId,
                MoveInDate = request.MoveInDate,
                MoveOutDate = request.MoveOutDate,
                StatusId = request.StatusId,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.Now,
                Status = request.Status,
            };

            var result = await _unitOfWork.CurrentResidentRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseCurrentResidentDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }
    }
}
