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
    public interface IWalletTransactionService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestWalletTransactionDto request);

    }
    public class WalletTransactionService : BaseService<WalletTransaction, ResponseWalletTransactionDto>, IWalletTransactionService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public WalletTransactionService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        //public async Task<ServiceResult> Search(string name)
        //{
        //    var WalletTransactions = await _unitOfWork.WalletTransactionRepository.GetByFullNameAsync(name);
        //    if (!WalletTransactions.Any())
        //    {
        //        return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, null);
        //    }
        //    else
        //    {
        //        var response = _mapper.Map<IEnumerable<ResponseWalletTransactionDto>>(WalletTransactions);
        //        return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, response, response.Count());
        //    }
        //}

        public async Task<ServiceResult> Create(RequestWalletTransactionDto request)
        {
            if (!await EntityExistsAsync("AccountId", request.AccountId))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            var createItem = new WalletTransaction()
            {
                AccountId = request.AccountId,
                Amount = request.Amount,
                TransactionTypeId = request.TransactionTypeId,
                TransactionStatusId = request.TransactionStatusId,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
            };

            var result = await _unitOfWork.WalletTransactionRepository.CreateAsync(createItem);
            var response = _mapper.Map<ResponseWalletTransactionDto>(result);

            return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, response);
        }

    }
}
