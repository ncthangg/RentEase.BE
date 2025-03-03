using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Authenticate;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data;
using RentEase.Data.Models;

namespace RentEase.Service.Service.Authenticate
{
    public interface IAuthenticateService
    {
        Task<ServiceResult> SignIn(RequestLoginDto request);
        Task<ServiceResult> SignUp(RequestRegisterDto request);
        Task<ServiceResult> ChangePassword(RequestChangePasswordDto request);
        Task<ServiceResult> GetInfo();
        Task<ServiceResult> Logout();
    }
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ServiceWrapper _serviceWrapper;
        private readonly HelperWrapper _helperWrapper;
        private readonly IAccountTokenService _accountTokenService;
        private readonly IAccountVerificationService _accountVerificationService;
        public AuthenticateService(
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ServiceWrapper serviceWrapper,
            HelperWrapper helperWrapper,
            IAccountTokenService accountTokenService,
            IAccountVerificationService accountVerificationService
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _serviceWrapper = serviceWrapper;
            _helperWrapper = helperWrapper;
            _accountTokenService = accountTokenService;
            _accountVerificationService = accountVerificationService;
        }

        public async Task<ServiceResult> SignIn(RequestLoginDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "UserName và Password không được để trống");
                }

                var accountExist = await _serviceWrapper.AccountService.GetByEmailOrPhoneAsync(request.Username);
                var accountData = _mapper.Map<Account>(accountExist.Data);
                if (accountData == null || !request.Password.Equals(accountData.PasswordHash) /*!_helperWrapper.PasswordHelper.VerifyPassword(requestLoginDto.Password, accountData.Passwordhash)*/)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Tài khoản không tồn tại hoặc Password không đúng");
                }

                if (!(bool)accountData.IsActive)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Tài khoản chưa được xác minh.");
                }

                // Tạo token
                var token = await _helperWrapper.TokenHelper.GenerateTokens(accountData.Id, accountData.RoleId);

                // Lưu Refresh token
                var saveTokenResult = await _accountTokenService.Save(accountData.Id, token.RefreshToken);
                if (saveTokenResult.Data == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lưu Refresh Token");
                }
                var tokenData = saveTokenResult.Data as AccountToken;

                // Xử lí DTO trả về
                var responseAccountDto = _mapper.Map<ResponseAccountDto>(accountData);
                responseAccountDto.ResponseWalletDto = (ResponseWalletDto)((await _serviceWrapper.WalletService.GetByIdAsync(accountData.Id)).Data);
                var responseAccountTokenDto = _mapper.Map<ResponseAccountToken>(tokenData);

                var response = new ResponseLoginDto
                {
                    ResponseAccountDto = responseAccountDto,
                    AccessToken = token.AccessToken,
                    ResponseAccountToken = responseAccountTokenDto,
                };

                return new ServiceResult(Const.SUCCESS_ACTION, "Login thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        public async Task<ServiceResult> SignUp(RequestRegisterDto request)
        {
            try
            {
                // Kiểm tra người dùng đã tồn tại chưa
                var itemExist = await _serviceWrapper.AccountService.GetByEmailOrPhoneAsync(request.Username);

                var itemData = _mapper.Map<Account>(itemExist.Data);
                if (itemData != null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Account đã tồn tại");
                }

                var response = new ResponseRegisterDto
                {
                    ResponseAccountDto = null,
                };

                var createItemResult = await _serviceWrapper.AccountService.CreateByGuest(request);
                if (createItemResult.Status < 0)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Register thất bại");
                }

                itemData = _mapper.Map<Account>(createItemResult.Data);
                if (itemData == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi Mapping");
                }

                if ((bool)itemData.IsActive)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Account đã tồn tại", response);
                }

                // Xử lí gửi Verify Code
                var verificationResult = await _accountVerificationService.HandleSendVerificationCode(itemData);
                if (verificationResult.Status < 0)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Gửi code thất bại");
                }

                response.ResponseAccountDto = _mapper.Map<ResponseAccountDto>(itemData);

                return new ServiceResult(Const.SUCCESS_ACTION, "Register thành công, Verification Code đã được gửi.", response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        public async Task<ServiceResult> GetInfo()
        {
            try
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

                var accountExist = await _serviceWrapper.AccountService.GetByIdAsync(accountIdInt);
                if (accountExist.Data == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Tài khoản không tồn tại ");
                }

                var walletExist = await _serviceWrapper.WalletService.GetByIdAsync(accountIdInt);
                if (walletExist.Data == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Ví không tồn tại ");
                }

                var responseAccountData = (ResponseAccountDto)accountExist.Data;
                var responseWalletData = (ResponseWalletDto)walletExist.Data;
                responseAccountData.ResponseWalletDto = responseWalletData;

                return new ServiceResult(Const.SUCCESS_ACTION, "Lấy thông tin tài khoản thành công", responseAccountData);

            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        public async Task<ServiceResult> ChangePassword(RequestChangePasswordDto request)
        {
            try
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

                // Kiểm tra người dùng đã tồn tại chưa
                 var item = await _unitOfWork.AccountRepository.GetByIdAsync(accountIdInt);
                if (item.PasswordHash.Equals(request.OldPassword) && request.NewPassword.Equals(request.ConfirmPassword))
                {
                    var updateItem = _mapper.Map<RequestAccountDto>(item);
                    updateItem.PasswordHash = request.NewPassword;
                    var result = await _serviceWrapper.AccountService.Update(accountIdInt, updateItem);
                    if(result.Status > 0)
                    {

                        return new ServiceResult(Const.SUCCESS_ACTION, "Thay đổi mật khẩu thành công");
                    }
                }

                return new ServiceResult(Const.ERROR_EXCEPTION, "Thay đổi mật khẩu thất bại");

            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }


        public Task<ServiceResult> Logout()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Clear(); // Xóa toàn bộ session

            // Trả về một thông báo khi đăng xuất thành công
            return Task.FromResult(new ServiceResult(Const.SUCCESS_ACTION, "Xóa Session thành công."));
        }

    }
}
