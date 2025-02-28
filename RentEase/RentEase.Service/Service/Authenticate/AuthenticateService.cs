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
        Task<ServiceResult> SignIn(RequestLoginDto request);
        Task<ServiceResult> SignUp(RequestRegisterDto request);
        //Task<ServiceResult> ChangePassword(RequestChangePasswordDto request);
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
                var responseAccountTokenDto = _mapper.Map<ResponseAccountToken>(tokenData);

                var response = new ResponseLoginDto
                {
                    ResponseAccountDto = responseAccountDto,
                    AccessToken = token.AccessToken,
                    ResponseAccountToken = responseAccountTokenDto,
                };

                return new ServiceResult(Const.SUCCESS_READ_CODE, "Login thành công", response);
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
                    ResponseAccountVerificationDto = null,
                };

                var createItemResult = await SaveAccount(request);
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
                response.ResponseAccountVerificationDto = _mapper.Map<ResponseAccountVerification>(verificationResult.Data);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, "Register thành công, Verification Code đã được gửi.", response);
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
                var accountData = _mapper.Map<Account>(accountExist.Data);
                if (accountData == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, "Tài khoản không tồn tại ");
                }

                var responseData = _mapper.Map<ResponseAccountDto>(accountData);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, responseData);

            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
            }
        }

        //public async Task<ServiceResult> ChangePassword(RequestChangePasswordDto request)
        //{
        //    try
        //    {
        //        string idUser = _helperWrapper.AuthenticateHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

        //        var response = new ResponseRegisterDto
        //        {
        //            ResponseAccountDto = null,
        //            ResponseAccountVerificationDto = null,
        //        };

        //        // Kiểm tra người dùng đã tồn tại chưa
        //        var itemExist = await _serviceWrapper.AccountService.GetByEmailOrPhoneAsync(request.Username);


        //        // Xử lí DTO trả về
        //        response.ResponseAccountDto = _mapper.Map<ResponseAccountDto>(itemData);
        //        response.ResponseAccountVerificationDto = _mapper.Map<ResponseAccountVerificationDto>(verificationResult.Data);

        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResult(Const.ERROR_EXCEPTION, ex.ToString());
        //    }
        //}

        private async Task<ServiceResult> SaveAccount(RequestRegisterDto request)
        {
            if (request.Password.Equals(request.ConfirmPassword))
            {
                //var hashedPassword = _helperWrapper.PasswordHelper.HashPassword(requestRegisterDto.Password);

                var isEmail = _serviceWrapper.AccountService.IsEmail(request.Username);
                var createItem = new Account();

                if (isEmail)
                {
                    createItem = new Account()
                    {
                        Email = request.Username,
                        FullName = request.Account.FullName,
                        PasswordHash = request.Account.PasswordHash,
                        PhoneNumber = request.Account.PhoneNumber,
                        DateOfBirth = request.Account.DateOfBirth,
                        Gender = request.Account.Gender,
                        AvatarUrl = request.Account.AvatarUrl,
                        RoleId = request.Account.RoleId,
                        IsActive = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = null,
                        DeletedAt = null,
                        Status = true,
                    };
                }
                else
                {
                    createItem = new Account()
                    {
                        Email = request.Account.Email,
                        FullName = request.Account.FullName,
                        PasswordHash = request.Account.PasswordHash,
                        PhoneNumber = request.Username,
                        DateOfBirth = request.Account.DateOfBirth,
                        Gender = request.Account.Gender,
                        AvatarUrl = request.Account.AvatarUrl,
                        RoleId = request.Account.RoleId,
                        IsActive = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = null,
                        DeletedAt = null,
                        Status = true,
                    };
                }

                var result = await _unitOfWork.AccountRepository.CreateAsync(createItem);
                if (result > 0)
                {
                    var responseData = _mapper.Map<ResponseAccountDto>(createItem);

                    return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
                }

                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
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
