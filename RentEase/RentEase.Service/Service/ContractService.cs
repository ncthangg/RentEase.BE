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
    public interface IContractService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestContractDto request);
        Task<ServiceResult> Update(int id, RequestContractDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class ContractService : BaseService<Contract, ResponseContractDto>, IContractService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public ContractService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(RequestContractDto request)
        {
            var createContract = new Contract()
            {
                AptId = request.AptId,
                LessorId = request.LessorId,
                LesseeId = request.LesseeId,
                AgentId = request.AgentId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RentPrice = request.RentPrice,
                PilePrice = request.PilePrice,
                FileUrl = request.FileUrl,
                ContractStatusId = request.ContractStatusId,
                ApproveStatusId = request.ApproveStatusId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.ContractRepository.CreateAsync(createContract);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseContractDto>(createContract);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestContractDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new Contract()
            {
                AptId = request.AptId,
                LessorId = request.LessorId,
                LesseeId = request.LesseeId,
                AgentId = request.AgentId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                RentPrice = request.RentPrice,
                PilePrice = request.PilePrice,
                FileUrl = request.FileUrl,
                ContractStatusId = request.ContractStatusId,
                ApproveStatusId = request.ApproveStatusId,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = request.DeletedAt,
                Status = request.Status,
            };

            var result = await _unitOfWork.ContractRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseContractDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }
    }
}
