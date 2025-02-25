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
    public interface IUtilityService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string? utilityName, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(RequestUtilityDto request);
        Task<ServiceResult> Update(int id, RequestUtilityDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);
    }
    public class UtilityService : BaseService<Utility, ResponseUtilityDto>, IUtilityService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public UtilityService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        //public async Task<ServiceResult> Search(string? utilityName, bool? status, int page, int pageSize)
        //{
        //    var items = await _unitOfWork.UtilityRepository.GetBySearchAsync(utilityName, status, page, pageSize);
        //    if (!items.Data.Any())
        //    {
        //        return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
        //    }
        //    else
        //    {
        //        var responseData = _mapper.Map<IEnumerable<ResponseUtilityDto>>(items.Data);
        //        return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, responseData);
        //    }
        //}

        public async Task<ServiceResult> Create(RequestUtilityDto request)
        {
            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            var createItem = new Utility()
            {
                Name = request.Name,
                CreatedAt = DateTime.UtcNow,
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

            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new Utility()
            {
                Name = request.Name,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
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

        public async Task<ServiceResult> Delete(int id)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }
            var item = (Utility)(await this.GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.UtcNow;
                item.Status = false;
            }

            var result = await _unitOfWork.UtilityRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseUtilityDto>(item);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }
    }
}
