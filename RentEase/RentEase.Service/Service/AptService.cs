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
    public interface IAptService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestAptDto request);
        Task<ServiceResult> Update(int id, RequestAptDto request);
        Task<ServiceResult> Delete(int id);

    }
    public class AptService : BaseService<Apt, ResponseAptDto>, IAptService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(RequestAptDto request)
        {
            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            var createItem = new Apt()
            {
                OwnerId = request.OwnerId,
                Name = request.Name,
                Description = request.Description,
                Area = request.Area,
                AddressLink = request.AddressLink,
                Address = request.Address,
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                WardId = request.WardId,
                RentPrice = request.RentPrice,
                PilePrice = request.PilePrice,
                CategoryId = request.CategoryId,
                StatusId = request.StatusId,
                NumberOfRoom = request.NumberOfRoom,
                AvailableRoom = request.AvailableRoom,
                ApproveStatusId = request.ApproveStatusId,
                Note = request.Note,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.AptRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptDto>(createItem);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestAptDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new Apt()
            {
                OwnerId = request.OwnerId,
                Name = request.Name,
                Description = request.Description,
                Area = request.Area,
                AddressLink = request.AddressLink,
                Address = request.Address,
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                WardId = request.WardId,
                RentPrice = request.RentPrice,
                PilePrice = request.PilePrice,
                CategoryId = request.CategoryId,
                StatusId = request.StatusId,
                NumberOfRoom = request.NumberOfRoom,
                AvailableRoom = request.AvailableRoom,
                ApproveStatusId = request.ApproveStatusId,
                Note = request.Note,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = null,
                Status = request.Status,
            };

            var result = await _unitOfWork.AptRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptDto>(updateItem);

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
            var item = (Apt)(await this.GetByIdAsync(id)).Data;

            if(item != null)
            {
                item.DeletedAt = DateTime.UtcNow;
                item.Status = false;
            }

            var result = await _unitOfWork.AptRepository.UpdateAsync(item);

            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptDto>(item);

                return new ServiceResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
        }
    }
}
