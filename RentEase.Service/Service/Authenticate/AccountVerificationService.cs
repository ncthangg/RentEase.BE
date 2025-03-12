using AutoMapper;
using Microsoft.Extensions.Configuration;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Authenticate;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;

namespace RentEase.Service.Service.Authenticate
{
    public interface IAccountVerificationService
    {
        Task<ServiceResult> GetByAccountId(string accountId);
        Task<ServiceResult> Save(string accountId, string verificationCode);
        Task<ServiceResult> Verification(string email, string verificationCode);
        Task<ServiceResult> HandleSendVerificationCode(string email);
    }
    public class AccountVerificationService : IAccountVerificationService
    {
        private readonly IConfiguration _configuration;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ServiceWrapper _serviceWrapper;
        private readonly HelperWrapper _helperWrapper;
        public AccountVerificationService(IConfiguration configuration, IMapper mapper, ServiceWrapper serviceWrapper, HelperWrapper helperWrapper)
        {
            _configuration = configuration;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _serviceWrapper = serviceWrapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> GetByAccountId(string accountId)
        {
            var account = await _unitOfWork.AccountVerificationRepository.GetByAccountId(accountId);
            if (account == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, account);
            }
        }

        public async Task<ServiceResult> Save(string accountId, string verificationCode)
        {
            var accountExist = await _unitOfWork.AccountRepository.EntityExistsByPropertyAsync("AccountId", accountId);
            if (accountExist)
            {
                var item = await _unitOfWork.AccountVerificationRepository.GetByAccountIdAndVerificationCode(accountId, verificationCode);

                if (item == null)
                {
                    var newCode = new AccountVerification()
                    {
                        AccountId = accountId,
                        VerificationCode = verificationCode,
                        CreatedAt = DateTime.Now,
                        ExpiresAt = DateTime.Now.AddDays(Convert.ToInt64(_configuration["SmtpSettings:VerificationTokenExpirationMinutes"])),
                        IsUsed = false,
                    };
                    await _unitOfWork.AccountVerificationRepository.CreateAsync(newCode);
                    return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Verification code created successfully", newCode);
                }
                else
                {
                    item.IsUsed = true;
                    await _unitOfWork.AccountVerificationRepository.UpdateAsync(item);
                    return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Verification code is valid");
                }
            }
            else
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
        }
        public async Task<ServiceResult> Verification(string email, string verificationCode)
        {
            var account = await _unitOfWork.AccountRepository.GetByEmailAsync(email);

            if (account == null)
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "User not found!");

            if ((bool)account.IsActive!)
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Account already verified!");

            bool isValid = await this.IsVerificationCodeValid(account.AccountId, verificationCode);

            if (!isValid)
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Invalid or expired verification code!");

            // Nếu hợp lệ, cập nhật trạng thái tài khoản
            account.IsActive = true;

            var resultUpdateAccount = await _unitOfWork.AccountRepository.UpdateAsync(account);
            if (resultUpdateAccount < 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Update account thất bại!");
            }

            var resultUpdateVerificationCode = await this.Save(account.AccountId, verificationCode);

            if (resultUpdateVerificationCode.Status < 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Update account thất bại!");
            }

            var responseData = new RegisterRes
            {
                FullName = account.FullName,
                Username = account.Email,
                RoleName = (await _unitOfWork.RoleRepository.GetByIdAsync(account.RoleId)).RoleName,
            };

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Account verified successfully!", responseData);

        }

        private async Task<bool> IsVerificationCodeValid(string accountId, string verificationCode)
        {
            var item = await _unitOfWork.AccountVerificationRepository.GetByAccountIdAndVerificationCode(accountId, verificationCode);

            if (item == null)
                return false; // Không có mã xác thực

            if (item.VerificationCode != null && item.VerificationCode != verificationCode)
                return false; // Mã không khớp

            if (item.ExpiresAt.ToShortTimeString() != null && item.ExpiresAt < DateTime.UtcNow)
                return false; // Mã đã hết hạn

            if (item.IsUsed != null && item.IsUsed == true)
                return false; // Mã đã sử dụng
            return true;
        }

        public async Task<ServiceResult> HandleSendVerificationCode(string email)
        {
            var item = await _unitOfWork.AccountRepository.GetByEmailAsync(email);

            var newVerificationCode = _helperWrapper.TokenHelper.GenerateVerificationCode();

            var saveResult = await this.Save(item.AccountId, newVerificationCode);
            if (saveResult.Status < 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Error saving verification code");
            }

            // Gửi email xác thực
            var verificationLink = $"https://yourdomain.com/verify?code={newVerificationCode}";
            await _helperWrapper.EmailHelper.SendVerificationEmailAsync(item.Email, newVerificationCode, verificationLink);

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Verification code sent", saveResult.Data);
        }


    }
}
