using AutoMapper;
using RentEase.Common.Base;
using RentEase.Data;
using RentEase.Data.Models;

namespace RentEase.Service.Service.Authenticate
{
    public interface IAccountVerificationService
    {
        Task<ServiceResult> GetByAccountId(int accountId);
        Task<ServiceResult> Save(int accountId, string verificationCode);
        Task<ServiceResult> CheckVerificationCodeValidity(int accountId, string verificationCode);
        Task<ServiceResult> HandleVerificationCode(Account account);
    }
    public class AccountVerificationService : IAccountVerificationService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ServiceWrapper _serviceWrapper;
        private readonly HelperWrapper _helperWrapper;
        public AccountVerificationService(IMapper mapper, ServiceWrapper serviceWrapper, HelperWrapper helperWrapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _serviceWrapper = serviceWrapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> GetByAccountId(int accountId)
        {
            var account = await _unitOfWork.AccountVerificationRepository.GetByAccountId(accountId);
            if (account == null)
            {
                return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, null);
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, account);
            }
        }
        public async Task<ServiceResult> Save(int accountId, string verificationCode)
        {
            var accountExist = await _serviceWrapper.AccountService.AccountExist(accountId);
            if (accountExist)
            {
                var code = await _unitOfWork.AccountVerificationRepository.GetByAccountId(accountId);

                if (code == null)
                {
                    var newCode = new AccountVerification()
                    {
                        AccountId = accountId,
                        VerificationCode = verificationCode,
                        CreatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                        IsUsed = false,
                    };
                    await _unitOfWork.AccountVerificationRepository.CreateAsync(newCode);
                    return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Verification code created successfully", newCode);
                }

                // Trường hợp mã đã hết hạn hoặc đã được sử dụng, cập nhật mã mới
                if (code.ExpiresAt < DateTime.UtcNow || (bool)code.IsUsed)
                {
                    code.VerificationCode = verificationCode;
                    code.CreatedAt = DateTime.UtcNow;
                    code.ExpiresAt = DateTime.UtcNow.AddMinutes(5);
                    code.IsUsed = false;  // Đảm bảo mã chưa sử dụng

                    await _unitOfWork.AccountVerificationRepository.UpdateAsync(code);
                    return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Verification code updated successfully", code);
                }

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Verification code is still valid", code);

            }
            else
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, null);
            }
        }
        public async Task<ServiceResult> CheckVerificationCodeValidity(int accountId, string verificationCode)
        {
            // Tìm AccountToken dựa trên AccountId và RefreshToken
            var code = await _unitOfWork.AccountVerificationRepository.GetByAccountId(accountId);

            // Kiểm tra nếu không tìm thấy mã xác thực
            if (code == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Verification code not found", null);
            }

            // Kiểm tra mã xác thực có khớp không
            if (!code.VerificationCode.Equals(verificationCode, StringComparison.OrdinalIgnoreCase))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Invalid verification code", null);
            }

            // Kiểm tra mã xác thực đã sử dụng chưa
            if ((bool)code.IsUsed)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Verification code has already been used", null);
            }

            // Kiểm tra thời gian hết hạn của mã xác thực
            if (code.ExpiresAt < DateTime.UtcNow)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Verification code has expired", null);
            }

            return new ServiceResult(Const.SUCCESS_READ_CODE, "Verification code is valid", code);
        }

        public async Task<ServiceResult> HandleVerificationCode(Account account)
        {

            var newVerificationCode = _helperWrapper.TokenHelper.GenerateVerificationCode();

            var saveResult = await this.Save(account.Id, newVerificationCode);
            if (saveResult.Status < 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Error saving verification code");
            }

            // Gửi email xác thực
            var verificationLink = $"https://yourdomain.com/verify?code={newVerificationCode}";
            await _helperWrapper.EmailHelper.SendVerificationEmailAsync(account.Email, newVerificationCode, verificationLink);

            return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Verification code sent", newVerificationCode);
        }


    }
}
