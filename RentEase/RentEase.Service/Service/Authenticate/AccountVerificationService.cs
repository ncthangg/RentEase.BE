using AutoMapper;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Authenticate;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;

namespace RentEase.Service.Service.Authenticate
{
    public interface IAccountVerificationService
    {
        Task<ServiceResult> GetByAccountId(int accountId);
        Task<ServiceResult> Save(int accountId, string verificationCode);
        Task<ServiceResult> Verification(int accountId, string verificationCode);
        Task<ServiceResult> HandleSendVerificationCode(Account account);
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

        public async Task<ServiceResult> GetByAccountId(int accountId)
        {
            var account = await _unitOfWork.AccountVerificationRepository.GetByAccountId(accountId);
            if (account == null)
            {
                return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
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
                    return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Verification code created successfully", newCode);
                }
                else
                {
                    item.IsUsed = true;
                    await _unitOfWork.AccountVerificationRepository.UpdateAsync(item);
                    return new ServiceResult(Const.SUCCESS_UPDATE_CODE, "Verification code is valid");
                }

            }
            else
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }
        }
        public async Task<ServiceResult> Verification(int accountId, string verificationCode)
        {
            var account = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);

            if (account == null)
                return new ServiceResult(Const.ERROR_EXCEPTION, "User not found!");

            if ((bool)account.IsActive)
                return new ServiceResult(Const.ERROR_EXCEPTION, "Account already verified!");

            bool isValid = await this.IsVerificationCodeValid(accountId, verificationCode);

            if (!isValid)
                return new ServiceResult(Const.ERROR_EXCEPTION, "Invalid or expired verification code!");

            // Nếu hợp lệ, cập nhật trạng thái tài khoản
            account.IsActive = true;
            var accountDto = _mapper.Map<RequestAccountDto>(account);
            var resultUpdateAccount = await _serviceWrapper.AccountService.Update(accountId, accountDto);
            if (resultUpdateAccount.Status < 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Update account thất bại!");
            }

            var responseAccount = _mapper.Map<ResponseAccountDto>(account);
            var resultUpdateVerificationCode = await this.Save(accountId, verificationCode);
            if (resultUpdateVerificationCode.Status < 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Update account thất bại!");
            }
            var createItemWallet = new Wallet()
            {
                AccountId = accountId,
                Balance = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };
            var result2 = await _unitOfWork.WalletRepository.CreateAsync(createItemWallet);
            if (result2 > 0)
            {
                var responseWallet = _mapper.Map<ResponseWalletDto>(createItemWallet);
                var responseData = new ResponseRegisterDto
                {
                    ResponseAccountDto = responseAccount,
                    ResponseWalletDto = responseWallet
                };
                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, "Account verified successfully!", responseData);
            }
            return new ServiceResult(Const.SUCCESS_UPDATE_CODE, "Account verified successfully!");
        }

        private async Task<bool> IsVerificationCodeValid(int accountId, string verificationCode)
        {
            var item = await _unitOfWork.AccountVerificationRepository.GetByAccountIdAndVerificationCode(accountId, verificationCode);

            if (item == null)
                return false; // Không có mã xác thực

            if (item.VerificationCode != null && item.VerificationCode != verificationCode)
                return false; // Mã không khớp

            if (item.ExpiresAt != null && item.ExpiresAt < DateTime.UtcNow)
                return false; // Mã đã hết hạn

            if (item.IsUsed != null && item.IsUsed == true)
                return false; // Mã đã sử dụng
            return true;
        }

        public async Task<ServiceResult> HandleSendVerificationCode(Account account)
        {

            var newVerificationCode = _helperWrapper.TokenHelper.GenerateVerificationCode();

            var saveResult = await this.Save(account.Id, newVerificationCode);
            if (saveResult.Status < 0)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Error saving verification code");
            }

            // Gửi email xác thực
            var verificationLink = $"https://yourdomain.com/verify?code={newVerificationCode}";
            //await _helperWrapper.EmailHelper.SendVerificationEmailAsync(account.Email, newVerificationCode, verificationLink);

            return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Verification code sent", saveResult.Data);
        }


    }
}
