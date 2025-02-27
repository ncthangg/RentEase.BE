using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service
{
    public interface IWalletService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestWalletDto request);
        Task<ServiceResult> Update(int id, RequestWalletDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class WalletService : BaseService<Wallet, ResponseWalletDto>, IWalletService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public WalletService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        //public async Task<ServiceResult> Search(string name)
        //{
        //    var Wallets = await _unitOfWork.WalletRepository.GetByFullNameAsync(name);
        //    if (!Wallets.Any())
        //    {
        //        return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, null);
        //    }
        //    else
        //    {
        //        var response = _mapper.Map<IEnumerable<ResponseWalletDto>>(Wallets);
        //        return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, response, response.Count());
        //    }
        //}

        public async Task<ServiceResult> Create(RequestWalletDto request)
        {
            if (await EntityExistsAsync("AccountId", request.AccountId))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            var createItem = new Wallet()
            {
                AccountId = request.AccountId,
                Balance = request.Balance,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.WalletRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseWalletDto>(result);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestWalletDto request)
        {
            if (!await EntityExistsAsync("AccountId", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            if (!await EntityExistsAsync("AccountId", request.AccountId))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }


            var updateItem = new Wallet()
            {
                AccountId = request.AccountId,
                Balance = request.Balance,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = request.DeletedAt,
                Status = request.Status,
            };

            var result = await _unitOfWork.WalletRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseWalletDto>(result);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }
    }
}
