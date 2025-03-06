using AutoMapper;
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
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetByEmail(string email);
        Task<ServiceResult> GetByPhone(string phoneNumber);
        Task<ServiceResult> GetByEmailOrPhone(string email);
        Task<ServiceResult> Search(string? fullName, string? email, string? phoneNumber, bool? isActive, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(AccountReq request);
        Task<ServiceResult> CreateByGuest(RegisterReq request);
        Task<ServiceResult> Update(string id, AccountReq request);
        Task<ServiceResult> Delete(string id);
        Task<ServiceResult> DeleteSoft(string id);
        Task<bool> AccountExistByMail(string email);
        Task<bool> AccountExistByPhoneNumber(string phoneNumber);
        bool IsEmail(string input);

    }
    public class AccountService : BaseService<Account, AccountRes>, IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AccountService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetByEmail(string email)
        {
            try
            {
                var item = await _unitOfWork.AccountRepository.GetByEmailAsync(email);
                if (item == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
                }

                var responseData = _mapper.Map<AccountRes>(item);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetByPhone(string phoneNumber)
        {
            try
            {
                var item = await _unitOfWork.AccountRepository.GetByPhoneAsync(phoneNumber);
                if (item == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
                }

                var responseData = _mapper.Map<AccountRes>(item);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetByEmailOrPhone(string username)
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

                var responseData = _mapper.Map<AccountRes>(item);
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
                var responseData = _mapper.Map<IEnumerable<AccountRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(AccountReq request)
        {
            if (await AccountExistByMail(request.Email) || await AccountExistByPhoneNumber(request.PhoneNumber))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            //var hashedPassword = _helperWrapper.PasswordHelper.HashPassword(request.PasswordHash);

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
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.AccountRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Tạo tài khoản thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> CreateByGuest(RegisterReq request)
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
                    return new ServiceResult(Const.SUCCESS_ACTION, "Tạo tài khoản thành công");
                }

                return new ServiceResult(Const.ERROR_EXCEPTION, "Tạo tài khoản thất bại");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(string id, AccountReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            var item = await _unitOfWork.AccountRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Không tồn tại");
            }

            if (accountId != item.AccountId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Bạn không có quyền hạn.");
            }

            if (!(bool)item.Status)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Status == False.");
            }

            var updateItem = new Account()
            {
                AccountId = item.AccountId,
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
                return new ServiceResult(Const.SUCCESS_ACTION, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> DeleteSoft(string id)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            var item = await _unitOfWork.AccountRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Không tồn tại");
            }

            if (accountId != item.AccountId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Bạn không có quyền hạn.");
            }

            item.DeletedAt = DateTime.Now;
            item.Status = false;

            var result = await _unitOfWork.AccountRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Xóa mềm thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        //CHECK
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
