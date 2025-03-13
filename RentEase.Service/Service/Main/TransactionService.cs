using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;
using RentEase.Service.Service.Payment;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RentEase.Service.Service.Main
{
    public interface ITransactionService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetByAccountId(string accountId, int? statusId, int page, int pageSize);
        Task<ServiceResult> GetById(int id);
    }

    public class TransactionService : BaseService<Transaction, TransactionRes>, ITransactionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public TransactionService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetByAccountId(string accountId, int? statusId, int page, int pageSize)
        {
            var items = await _unitOfWork.TransactionRepository.GetByAccountId(accountId, statusId, page, pageSize);

            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<TransactionRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }

        
    }
}
