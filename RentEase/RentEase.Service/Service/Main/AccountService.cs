using AutoMapper;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Authenticate;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IAccountService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> GetByEmailAsync(string email);
        Task<ServiceResult> GetByPhoneAsync(string phoneNumber);
        Task<ServiceResult> GetByEmailOrPhoneAsync(string email);
        Task<ServiceResult> Search(string? fullName, string? email, string? phoneNumber, bool? isActive, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(RequestAccountDto request);
        Task<ServiceResult> CreateByGuest(RequestRegisterDto request);
        Task<ServiceResult> Update(int id, RequestAccountDto request);
        Task<ServiceResult> Delete(int id);
        Task<bool> AccountExist(int id);
        Task<bool> AccountExistByMail(string email);
        Task<bool> AccountExistByPhoneNumber(string phoneNumber);
        bool IsEmail(string input);

    }
    public class AccountService : BaseService<Account, ResponseAccountDto>, IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        private readonly IWalletService _walletService;
        public AccountService(IMapper mapper, HelperWrapper helperWrapper, IWalletService walletService)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
            _walletService = walletService;
        }
        public async Task<ServiceResult> GetByEmailAsync(string email)
        {
            try
            {
                var item = await _unitOfWork.AccountRepository.GetByEmailAsync(email);
                if (item == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
                }

                var responseData = _mapper.Map<ResponseAccountDto>(item);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetByPhoneAsync(string phoneNumber)
        {
            try
            {
                var item = await _unitOfWork.AccountRepository.GetByPhoneAsync(phoneNumber);
                if (item == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
                }

                var responseData = _mapper.Map<ResponseAccountDto>(item);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetByEmailOrPhoneAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
                }

                var item = await _unitOfWork.AccountRepository.GetByEmailOrPhoneAsync(username);
                if (item == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
                }

                var responseData = _mapper.Map<ResponseAccountDto>(item);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }

        public async Task<ServiceResult> Search(string? fullName, string? email, string? phoneNumber, bool? isActive, bool? status, int page, int pageSize)
        {
            var items = await _unitOfWork.AccountRepository.GetBySearchAsync(fullName, email, phoneNumber, isActive, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAccountDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }

        public async Task<ServiceResult> Create(RequestAccountDto request)
        {
            if (await AccountExistByMail(request.Email) || await AccountExistByPhoneNumber(request.PhoneNumber))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new Account()
            {
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = request.PasswordHash,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                AvatarUrl = request.AvatarUrl,
                RoleId = request.RoleId,
                IsActive = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result1 = await _unitOfWork.AccountRepository.CreateAsync(createItem);
            if (result1 > 0)
            {
                var responseData1 = _mapper.Map<ResponseAccountDto>(createItem);
                var createItemWallet = new Wallet()
                {
                    AccountId = responseData1.Id,
                    Balance = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    DeletedAt = null,
                    Status = true,
                };
                var result2 = await _unitOfWork.WalletRepository.CreateAsync(createItemWallet);
                if (result2 > 0)
                {
                    var responseWalletData = _mapper.Map<ResponseWalletDto>(createItemWallet);
                    responseData1.ResponseWalletDto = responseWalletData;
                    return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData1);
                }

                return new ServiceResult(Const.SUCCESS_ACTION, "Tạo tài khoản thành công Nhưng Tạo ví thất bại");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> CreateByGuest(RequestRegisterDto request)
        {
            if (request.Password.Equals(request.ConfirmPassword))
            {
                //var hashedPassword = _helperWrapper.PasswordHelper.HashPassword(requestRegisterDto.Password);

                var isEmail = IsEmail(request.Username);
                var createItem = new Account();

                if (isEmail)
                {
                    createItem = new Account()
                    {
                        Email = request.Username,
                        FullName = null,
                        PasswordHash = request.Password,
                        PhoneNumber = null,
                        DateOfBirth = null,
                        Gender = null,
                        AvatarUrl = null,
                        RoleId = request.RoleId,
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
                        Email = null,
                        FullName = null,
                        PasswordHash = request.Password,
                        PhoneNumber = request.Username,
                        DateOfBirth = null,
                        Gender = null,
                        AvatarUrl = null,
                        RoleId = request.RoleId,
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

                    return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
                }

                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(int id, RequestAccountDto request)
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

            if (!await AccountExist(id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            var item = _mapper.Map<Account>((ResponseAccountDto)(await GetByIdAsync(id)).Data);

            var updateItem = new Account()
            {
                Id = id,
                Email = item.Email,
                FullName = request.FullName,
                PasswordHash = request.PasswordHash,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                AvatarUrl = request.AvatarUrl,
                RoleId = request.RoleId,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = null,
                Status = item.Status,
            };

            var result = await _unitOfWork.AccountRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAccountDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Delete(int id)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            var itemAccount = (Account)(await GetByIdAsync(id)).Data;

            if (itemAccount != null)
            {
                itemAccount.DeletedAt = DateTime.Now;
                itemAccount.Status = false;
            }

            var result = await _unitOfWork.AccountRepository.UpdateAsync(itemAccount);
            if (result > 0)
            {
                var itemWallet = await _walletService.Delete(id);

                if (itemWallet.Status < 0)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
                }

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<bool> AccountExist(int id)
        {
            return await _unitOfWork.AccountRepository.EntityExistsByPropertyAsync("Id", id);
        }
        public async Task<bool> AccountExistByMail(string email)
        {
            return await _unitOfWork.AccountRepository.EntityExistsByPropertyAsync("Email", email);
        }
        public async Task<bool> AccountExistByPhoneNumber(string phoneNumber)
        {
            return await _unitOfWork.AccountRepository.EntityExistsByPropertyAsync("PhoneNumber", phoneNumber);
        }

        public bool IsEmail(string input)
        {
            return input.Contains("@");
        }
    }

}
