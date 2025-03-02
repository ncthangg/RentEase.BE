
using AutoMapper;
using Microsoft.Extensions.Configuration;
using RentEase.Common.Base;
using RentEase.Data;
using RentEase.Data.Models;

namespace RentEase.Service.Service.Authenticate
{
    public interface IAccountTokenService
    {
        Task<ServiceResult> Save(int accountId, string refreshToken);
        Task<ServiceResult> CheckRefreshTokenValidity(int id, string refreshToken);
    }
    public class AccountTokenService : IAccountTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ServiceWrapper _serviceWrapper;
        private readonly HelperWrapper _helperWrapper;
        public AccountTokenService(IConfiguration configuration,
                                  IMapper mapper,
                                  ServiceWrapper serviceWrapper,
                                  HelperWrapper helperWrapper)
        {
            _configuration = configuration;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _serviceWrapper = serviceWrapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> Save(int accountId, string refreshToken)
        {
            if (accountId != null)
            {
                var newToken = new AccountToken()
                {
                    AccountId = accountId,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.Now.AddDays(Convert.ToInt32(_configuration["JwtSettings:RefreshTokenExpirationDays"])),
                    CreatedAt = DateTime.Now
                };
                await _unitOfWork.AccountTokenRepository.CreateAsync(newToken);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, newToken);
            }
            else
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
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
            if (accountToken.ExpiresAt < DateTime.Now)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Refresh token has expired");
            }

            // Nếu token còn hạn
            return new ServiceResult(Const.SUCCESS_ACTION, "Refresh token is valid", accountToken);
        }
    }
}
