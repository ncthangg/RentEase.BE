using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IAccountLikedAptService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? statuss);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetByAccountId(string accountId, int page, int pageSize);
        Task<ServiceResult> Create(string accountId, string aptId);
        Task<ServiceResult> Remove(string accountId, string aptId);
        Task<ServiceResult> RemoveAll(string accountId);
    }
    public class AccountLikedAptService : BaseService<AccountLikedApt, AccountLikedAptRes>, IAccountLikedAptService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AccountLikedAptService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetByAccountId(string accountId, int page, int pageSize)
        {
            var items = await _unitOfWork.AccountLikedAptRepository.GetByAccountId(accountId, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<AccountLikedAptRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(string accountId, string aptId)
        {
            var item = new AccountLikedApt()
            {
                AccountId = accountId,
                AptId = aptId,
                CreatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.AccountLikedAptRepository.CreateAsync(item);

            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Remove(string accountId, string aptId)
        {
            var result = await _unitOfWork.AccountLikedAptRepository.RemoveByAccountIdAndAptIdAsync(accountId, aptId);

            if (result)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> RemoveAll(string accountId)
        {
            var result = await _unitOfWork.AccountLikedAptRepository.RemoveAllByAccountIdAsync(accountId);

            if (result)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
