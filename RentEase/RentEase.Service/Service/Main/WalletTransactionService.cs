using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IWalletTransactionService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
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

        public async Task<ServiceResult> Create(RequestWalletTransactionDto request)
        {

            var createItem = new WalletTransaction()
            {
                WalletId = request.WalletId,
                Amount = request.Amount,
                TransactionTypeId = request.TransactionTypeId,
                TransactionStatusId = request.TransactionStatusId,
                Description = request.Description,
                CreatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.WalletTransactionRepository.CreateAsync(createItem);
            var response = _mapper.Map<ResponseWalletTransactionDto>(result);

            return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, response);
        }

    }
}
