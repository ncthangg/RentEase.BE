using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data.Models;
using RentEase.Data;
using RentEase.Service.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Service.Service
{
    public interface IAptStatusService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestAptStatusDto request);
        Task<ServiceResult> Update(int id, RequestAptStatusDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class AptStatusService : BaseService<AptStatus, ResponseAptStatusDto>, IAptStatusService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptStatusService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(RequestAptStatusDto request)
        {
            if (await EntityExistsAsync("StatusName", request.StatusName))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            var createItem = new AptStatus()
            {
                StatusName = request.StatusName,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.AptStatusRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptStatusDto>(createItem);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestAptStatusDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            if (await EntityExistsAsync("StatusName", request.StatusName))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new AptStatus()
            {
                StatusName = request.StatusName,
                Description = request.Description,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = null,
                Status = request.Status,
            };

            var result = await _unitOfWork.AptStatusRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptStatusDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }

    }
}
