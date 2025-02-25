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

    public interface IRoleService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestRoleDto request);
        Task<ServiceResult> Update(int id, RequestRoleDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class RoleService : BaseService<Role, ResponseRoleDto>, IRoleService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public RoleService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(RequestRoleDto request)
        {
            if (await EntityExistsAsync("RoleName", request.RoleName))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            var createItem = new Role()
            {
                RoleName = request.RoleName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
            };

            var result = await _unitOfWork.RoleRepository.CreateAsync(createItem); 
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseRoleDto>(createItem);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestRoleDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            if (await EntityExistsAsync("RoleName", request.RoleName))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new Role()
            {
                RoleName = request.RoleName,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _unitOfWork.RoleRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseRoleDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }
    }
}
