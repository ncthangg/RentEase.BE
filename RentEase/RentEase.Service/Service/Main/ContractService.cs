using AutoMapper;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;
using System.Net.NetworkInformation;
using static RentEase.Common.Base.EnumType;

namespace RentEase.Service.Service.Main
{
    public interface IContractService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Create(RequestContractDto request);
        Task<ServiceResult> Update(int id, RequestContractDto request, int? contractStatus, int? approveStatus);
        Task<ServiceResult> Delete(int id);

    }
    public class ContractService : BaseService<Contract, ResponseContractDto>, IContractService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
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
                ContractStatusId = (int)EnumType.ContractStatus.Pending,
                ApproveStatusId = (int)EnumType.ApproveStatus.Pending,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.ContractRepository.CreateAsync(createContract);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseContractDto>(createContract);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestContractDto request, int? contractStatus, int? approveStatus)
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

            var item = _mapper.Map<Contract>((ResponseContractDto)(await GetByIdAsync(id)).Data);

            if (approveStatus != (int)EnumType.ApproveStatus.Pending &&
                    approveStatus != (int)EnumType.ApproveStatus.Approved &&
                        approveStatus != (int)EnumType.ApproveStatus.Rejected)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "ApproveStatus không hợp lệ.");
            }

            if (contractStatus != (int)EnumType.ContractStatus.Pending &&
                    contractStatus != (int)EnumType.ContractStatus.Active &&
                         contractStatus != (int)EnumType.ContractStatus.ExpiringSoon &&
                              contractStatus != (int)EnumType.ContractStatus.Completed &&
                                     contractStatus != (int)EnumType.ContractStatus.Cancelled)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "ContractStatus không hợp lệ.");
            }

            if (accountIdInt == 1)
            {
                item.ContractStatusId = (int)contractStatus;
                item.ApproveStatusId = (int)approveStatus;
                item.UpdatedAt = DateTime.Now;

                var result1 = await _unitOfWork.ContractRepository.UpdateAsync(item);
                if (result1 > 0)
                {
                    var responseData = _mapper.Map<ResponseAptDto>(item);

                    return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
                }
            }

            var updateItem = new Contract()
            {
                Id = id,
                AptId = item.AptId,
                LessorId = item.LessorId,
                LesseeId = item.LesseeId,
                AgentId = item.AgentId,
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                RentPrice = item.RentPrice,
                PilePrice = item.PilePrice,
                FileUrl = item.FileUrl,
                ContractStatusId = (int)contractStatus,
                ApproveStatusId = (int)approveStatus,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = item.DeletedAt,
                Status = item.Status,
            };

            var result = await _unitOfWork.ContractRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseContractDto>(updateItem);

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
            var item = (Contract)(await GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.ContractRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseContractDto>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
