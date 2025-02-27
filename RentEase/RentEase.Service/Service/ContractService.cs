using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service
{
    public interface IContractService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestContractDto request);
        Task<ServiceResult> Update(int id, RequestContractDto request);
        Task<ServiceResult> Delete(int id);

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
                CreatedAt = DateTime.Now,
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
                UpdatedAt = DateTime.Now,
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

        public async Task<ServiceResult> Delete(int id)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
            }
            var item = (Contract)(await this.GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.ContractRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseContractDto>(item);

                return new ServiceResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
        }
    }
}
