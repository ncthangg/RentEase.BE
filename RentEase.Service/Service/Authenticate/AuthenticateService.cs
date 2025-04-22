using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Authenticate;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Helper;

namespace RentEase.Service.Service.Authenticate
{
    public interface IAuthenticateService
    {
        Task<ServiceResult> SignIn(LoginReq request);
        Task<ServiceResult> SignUp(RegisterReq request);
        Task<ServiceResult> ChangePassword(string accountId, ChangePasswordReq request);
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
        private readonly IPasswordHelper _passwordHelper;
        public AuthenticateService(
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            ServiceWrapper serviceWrapper,
            HelperWrapper helperWrapper,
            IAccountTokenService accountTokenService,
            IAccountVerificationService accountVerificationService,
            IPasswordHelper passwordHelper
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _serviceWrapper = serviceWrapper;
            _helperWrapper = helperWrapper;
            _accountTokenService = accountTokenService;
            _accountVerificationService = accountVerificationService;
            _passwordHelper = passwordHelper;
        }

        public async Task<ServiceResult> SignIn(LoginReq request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "UserName và Password không được để trống");
                }
                var account = await _unitOfWork.AccountRepository.GetByEmailOrPhoneAsync(request.Username);

                if (account == null || !_helperWrapper.PasswordHelper.VerifyPassword(request.Password, account.PasswordHash))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tài khoản không tồn tại hoặc Password không đúng");
                }

                if (!(bool)account.IsVerify!)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tài khoản chưa được xác minh.");
                }
                if (!(bool)account.Status!)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tài khoản sẽ xóa sau 7 ngày.");
                }

                // Tạo token
                var token = _helperWrapper.TokenHelper.GenerateJwtTokens(account.AccountId, account.RoleId);

                // Lưu Refresh token
                var saveTokenResult = await _accountTokenService.Save(account.AccountId, token.RefreshToken);
                if (saveTokenResult.Data == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lưu Refresh Token");
                }
                var tokenData = saveTokenResult.Data as AccountToken;

                // Xử lí DTO trả về
                var responseAccountDto = _mapper.Map<AccountRes>(account);
                var responseAccountTokenDto = _mapper.Map<AccountTokenRes>(tokenData);

                var response = new LoginRes
                {
                    AccountRes = responseAccountDto,
                    AccessToken = token.AccessToken,
                    AccountTokenRes = responseAccountTokenDto,
                };

                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Login thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, ex.ToString());
            }
        }
        public async Task<ServiceResult> SignUp(RegisterReq request)
        {
            try
            {
                var createItemResult = await _serviceWrapper.AccountService.CreateByGuest(request);
                if (createItemResult.Status < 0)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, createItemResult.Message);
                }
                var createItemResultData = (Account)createItemResult.Data!;

                // Xử lí gửi Verify Code
                var verificationResult = await _accountVerificationService.HandleSendVerificationCode(request.Username);
                if (verificationResult.Status < 0)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, verificationResult.Message);
                }

                var response = new RegisterRes
                {
                    FullName = createItemResultData!.FullName,
                    Username = createItemResultData.Email,
                    RoleName = (await _unitOfWork.RoleRepository.GetByIdAsync(createItemResultData.RoleId)).RoleName,
                };

                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Register thành công, Verification Code đã được gửi.", response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, ex.ToString());
            }
        }
        public async Task<ServiceResult> GetInfo()
        {
            try
            {
                string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

                if (string.IsNullOrEmpty(accountId))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
                }

                var item = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);
                if (item == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tài khoản không tồn tại ");
                }

                var data = _mapper.Map<AccountRes>((item as Account));
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Lấy thông tin tài khoản thành công", data);

            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, ex.ToString());
            }
        }
        public async Task<ServiceResult> ChangePassword(string id, ChangePasswordReq request)
        {
            try
            {
                string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
                string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

                if (string.IsNullOrEmpty(accountId))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
                }

                // Kiểm tra người dùng đã tồn tại chưa
                var item = await _unitOfWork.AccountRepository.GetByIdAsync(id);

                if (accountId != item.AccountId && roleId != "1")
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
                }

                if (!(bool)item.Status!)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Status == False.");
                }

                if (_passwordHelper.VerifyPassword(request.OldPassword, item.PasswordHash) && request.NewPassword.Equals(request.ConfirmPassword))
                {
                    var passwordHash = _passwordHelper.HashPassword(request.NewPassword);
                    var result = await _serviceWrapper.AccountService.UpdatePassword(id, passwordHash);
                    if (result.Status > 0)
                    {
                        return new ServiceResult(Const.SUCCESS_ACTION_CODE, result.Message);
                    }
                }

                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Thay đổi mật khẩu thất bại");

            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, ex.ToString());
            }
        }

        public Task<ServiceResult> Logout()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Clear(); // Xóa toàn bộ session

            // Trả về một thông báo khi đăng xuất thành công
            return Task.FromResult(new ServiceResult(Const.SUCCESS_ACTION_CODE, "Xóa Session thành công."));
        }

    }
}
