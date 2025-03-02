using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;
using static RentEase.Common.Base.EnumType;

namespace RentEase.Service.Service.Main
{
    public interface IWalletTransactionService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Create(RequestWalletTransactionDto request);
        Task<ServiceResult> Update(int id, int? transactionStatus);

    }
    public class WalletTransactionService : BaseService<WalletTransaction, ResponseWalletTransactionDto>, IWalletTransactionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public WalletTransactionService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(RequestWalletTransactionDto request)
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

            var createItem = new WalletTransaction()
            {
                WalletId = accountIdInt,
                OrderId = request.OrderId,
                Amount = request.Amount,
                TransactionTypeId = request.TransactionTypeId,
                TransactionStatusId = (int)EnumType.TransactionStatus.Pending,
                Description = request.Description,
                CreatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.WalletTransactionRepository.CreateAsync(createItem);
            var response = _mapper.Map<ResponseWalletTransactionDto>(result);

            return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, response);
        }

        public async Task<ServiceResult> Update(int id, int? transactionStatus)
        {

            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = (WalletTransaction)(await GetByIdAsync(id)).Data;
            if (transactionStatus != (int)EnumType.TransactionStatus.Pending &&
                    transactionStatus != (int)EnumType.TransactionStatus.Success &&
                        transactionStatus != (int)EnumType.TransactionStatus.Failed)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "ProgressStatus không hợp lệ.");
            }
            item.TransactionStatusId = (int)transactionStatus;

            var result = await _unitOfWork.WalletTransactionRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<WalletTransaction>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

    }
}
