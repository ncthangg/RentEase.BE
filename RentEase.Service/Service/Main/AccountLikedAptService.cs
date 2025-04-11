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
        //Task<ServiceResult> GetAll(int page, int pageSize, bool? statuss);
        //Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetByAccountId(int page, int pageSize);
        Task<ServiceResult> Create(string aptId);
        Task<ServiceResult> Remove(string aptId);
        Task<ServiceResult> RemoveAll();
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
        public async Task<ServiceResult> GetByAccountId(int page, int pageSize)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var items = await _unitOfWork.AccountLikedAptRepository.GetByAccountId(accountId, true, page, pageSize);
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
        public async Task<ServiceResult> Create(string aptId)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }
            var apt = await _unitOfWork.AptRepository.GetByIdAsync(aptId);

            if (!(bool)apt.Status)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Apt không tồn tại");
            }

            var item = new AccountLikedApt()
            {
                AccountId = accountId,
                AptId = aptId,
                CreatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.AccountLikedAptRepository.CreateAsync(item);

            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Like thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Remove(string aptId)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var result = await _unitOfWork.AccountLikedAptRepository.RemoveByAccountIdAndAptIdAsync(accountId, aptId);

            if (result)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Remove thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> RemoveAll()
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var result = await _unitOfWork.AccountLikedAptRepository.RemoveAllByAccountIdAsync(accountId);

            if (result)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "RemoveAll thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
