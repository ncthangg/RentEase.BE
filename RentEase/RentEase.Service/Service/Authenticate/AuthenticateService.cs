using AutoMapper;
using Microsoft.AspNetCore.Http;
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
        Task<ServiceResult> Login(RequestLoginDto requestLoginDto);
        Task<ServiceResult> Register(RequestRegisterDto requestRegisterDto);
        Task<ServiceResult> Logout();
    }
    public class AuthenticateService : IAuthenticateService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ServiceWrapper _serviceWrapper;
        private readonly HelperWrapper _helperWrapper;
        private readonly IAccountTokenService _accountTokenService;
        private readonly IAccountVerificationService _accountVerificationService;
        public AuthenticateService(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            ServiceWrapper serviceWrapper,
            HelperWrapper helperWrapper,
            IAccountTokenService accountTokenService,
            IAccountVerificationService accountVerificationService
            )
        {
            _unitOfWork ??= new UnitOfWork();
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _serviceWrapper = serviceWrapper;
            _helperWrapper = helperWrapper;
            _accountTokenService = accountTokenService;
            _accountVerificationService = accountVerificationService;
        }

        public async Task<ServiceResult> Login(RequestLoginDto requestLoginDto)
        {
            try
            {
                if (string.IsNullOrEmpty(requestLoginDto.Username) || string.IsNullOrEmpty(requestLoginDto.Password))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Email và Password không được để trống", null);
                }

                var accountExist = await _serviceWrapper.AccountService.GetByEmailAsync(requestLoginDto.Username);
                var accountData = accountExist.Data as Account;
                if (accountData == null || !requestLoginDto.Password.Equals(accountData.PasswordHash) /*!_helperWrapper.PasswordHelper.VerifyPassword(requestLoginDto.Password, accountData.Passwordhash)*/)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Account không tồn tại hoặc Password không đúng", null);
                }

                if (!(bool)accountData.IsActive)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Tài khoản chưa được xác minh.", null);
                }

                // Tạo token
                var token = _helperWrapper.TokenHelper.GenerateJWT(accountData, accountData.Role.RoleName);
                var refreshToken = _helperWrapper.TokenHelper.GenerateRefreshToken();

                // Lưu Refresh token
                var saveTokenResult = await _accountTokenService.Save(accountData.Id, refreshToken);
                if (saveTokenResult.Data != null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lưu Refresh Token", null);
                }
                var tokenData = saveTokenResult.Data as AccountToken;

                // Xử lí DTO trả về
                var responseAccountDto = _mapper.Map<ResponseAccountDto>(accountData);
                var responseAccountTokenDto = _mapper.Map<ResponseAccountTokenDto>(tokenData);

                var response = new ResponseLoginDto
                {
                    ResponseAccountDto = responseAccountDto,
                    ResponseAccountTokenDto = responseAccountTokenDto,
                    Token = token
                };

                return new ServiceResult(Const.SUCCESS_READ_CODE, "Login thành công", response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }
        public async Task<ServiceResult> Register(RequestRegisterDto request)
        {
            try
            {
                // Kiểm tra người dùng đã tồn tại chưa
                var accountExist = await _serviceWrapper.AccountService.GetByEmailAsync(request.Username);

                var accountData = new Account();
                if (accountExist.Data != null)
                {
                    accountData = accountExist.Data as Account;
                }

                var response = new ResponseRegisterDto
                {
                    ResponseAccountDto = null,
                    ResponseAccountVerificationDto = null,
                };

                if (accountData != null)
                {
                    // Người dùng đã tồn tại -> Response
                    if ((bool)accountData.IsActive)
                    {
                        response.ResponseAccountDto = _mapper.Map<ResponseAccountDto>(accountData);
                        return new ServiceResult(Const.ERROR_EXCEPTION, "Account đã tồn tại", response);
                    }

                }
                else
                {
                    var createAccountResult = await AddAccount(request);
                    if (createAccountResult.Status < 0)
                    {
                        return new ServiceResult(Const.ERROR_EXCEPTION, "Register thất bại");
                    }
                    accountData = createAccountResult.Data as Account;
                    if (accountData == null)
                    {
                        return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi Mapping");
                    }
                }

                // Xử lí gửi Verify Code
                var verificationResult = await _accountVerificationService.HandleVerificationCode(accountData);
                if (verificationResult.Status < 0)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Gửi code thất bại");
                }

                // Xử lí DTO trả về
                response.ResponseAccountDto = _mapper.Map<ResponseAccountDto>(accountData);
                response.ResponseAccountVerificationDto = _mapper.Map<ResponseAccountVerificationDto>(verificationResult.Data);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Register thành công, Verification Code đã được gửi.", response);
            }
            catch (Exception ex)
            {
                // Log lỗi tại đây nếu cần
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        private async Task<ServiceResult> AddAccount(RequestRegisterDto requestRegisterDto)
        {
            if (requestRegisterDto.Password.Equals(requestRegisterDto.ConfirmPassword))
            {
                var hashedPassword = _helperWrapper.PasswordHelper.HashPassword(requestRegisterDto.Password);

                //var role = await _serviceWrapper.RoleService.GetByRoleName("member");
                //var roleId = (role.Data as Role).Id;
                var roleId = 1;

                var newAccount = new RequestAccountDto()
                {
                    Email = requestRegisterDto.Username,
                    PasswordHash = hashedPassword,
                    RoleId = roleId,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow,
                };
                await _serviceWrapper.AccountService.Create(newAccount);
                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, newAccount);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG, null);

        }
        public Task<ServiceResult> Logout()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.Clear(); // Xóa toàn bộ session

            // Trả về một thông báo khi đăng xuất thành công
            return Task.FromResult(new ServiceResult(Const.SUCCESS_READ_CODE, "Xóa Session thành công."));
        }

    }
}
