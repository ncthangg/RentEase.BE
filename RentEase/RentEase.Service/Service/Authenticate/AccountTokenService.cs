
using AutoMapper;
using RentEase.Common.Base;
using RentEase.Data;
using RentEase.Data.Models;

namespace RentEase.Service.Service.Authenticate
{
    public interface IAccountTokenService
    {
        Task<ServiceResult> Save(int id, string refreshToken);
        Task<ServiceResult> CheckRefreshTokenValidity(int id, string refreshToken);
    }
    public class AccountTokenService : IAccountTokenService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ServiceWrapper _serviceWrapper;
        private readonly HelperWrapper _helperWrapper;
        public AccountTokenService(IMapper mapper, ServiceWrapper serviceWrapper, HelperWrapper helperWrapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _serviceWrapper = serviceWrapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> Save(int id, string refreshToken)
        {
            var accountExist = await _serviceWrapper.AccountService.AccountExist(id);
            if (accountExist)
            {
                var newToken = new AccountToken()
                {
                    AccountId = id,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.AccountTokenRepository.CreateAsync(newToken);
                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, newToken);
            }
            else
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }
        }

        public async Task<ServiceResult> CheckRefreshTokenValidity(int id, string refreshToken)
        {
            // Tìm AccountToken dựa trên AccountId và RefreshToken
            var accountToken = await _unitOfWork.AccountTokenRepository.GetByAccountIdAndToken(id, refreshToken);

            // Kiểm tra nếu không tìm thấy token trong cơ sở dữ liệu
            if (accountToken == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Refresh token not found");
            }

            // Kiểm tra thời gian hết hạn của RefreshToken
            if (accountToken.ExpiresAt.HasValue && accountToken.ExpiresAt.Value < DateTime.UtcNow)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Refresh token has expired");
            }

            // Nếu token còn hạn
            return new ServiceResult(Const.SUCCESS_READ_CODE, "Refresh token is valid", accountToken);
        }
    }
}
