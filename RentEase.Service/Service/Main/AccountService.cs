using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Client;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Authenticate;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;
using System.Text.RegularExpressions;

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
        Task<ServiceResult> Create(PostAccountReq request);
        Task<ServiceResult> CreateByGuest(RegisterReq request);
        Task<ServiceResult> Update(string id, PutAccountReq request);
        Task<ServiceResult> UpdatePassword(string id, string newPassword);
        Task<ServiceResult> UpdateStatus(string id);
        Task<ServiceResult> Delete(string id);
        Task<bool> AccountExistByMail(string email);
        Task<bool> AccountExistByPhoneNumber(string phoneNumber);

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
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
                }

                var responseData = _mapper.Map<AccountRes>(item);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetByPhone(string phoneNumber)
        {
            try
            {
                var item = await _unitOfWork.AccountRepository.GetByPhoneAsync(phoneNumber);
                if (item == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
                }

                var responseData = _mapper.Map<AccountRes>(item);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetByEmailOrPhone(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
                }

                var item = await _unitOfWork.AccountRepository.GetByEmailOrPhoneAsync(username);
                if (item == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
                }

                var responseData = _mapper.Map<AccountRes>(item);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> Search(string? fullName, string? email, string? phoneNumber, bool? isActive, bool? status, int page, int pageSize)
        {
            var items = await _unitOfWork.AccountRepository.GetBySearchAsync(fullName, email, phoneNumber, isActive, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<AccountRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(PostAccountReq request)
        {
            if (await AccountExistByMail(request.Email) || await AccountExistByPhoneNumber(request.PhoneNumber))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var hashedPassword = _helperWrapper.PasswordHelper.HashPassword(request.Password);

            var createItem = new Account()
            {
                AccountId = Guid.NewGuid().ToString("N"),
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = hashedPassword,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                GenderId = request.GenderId,
                AvatarUrl = request.AvatarUrl,
                RoleId = request.RoleId,
                PublicPostTimes = 0,
                IsVerify = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            if (request.RoleId == 2)
            {
                createItem.PublicPostTimes = 1;
            }

            var result = await _unitOfWork.AccountRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo tài khoản thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> CreateByGuest(RegisterReq request)
        {
            var itemExist = await this.GetByEmailOrPhone(request.Username);

            var itemData = _mapper.Map<Account>(itemExist.Data);
            if (itemData != null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Account đã tồn tại");
            }

            if (request.Password.Equals(request.ConfirmPassword))
            {
                var hashedPassword = _helperWrapper.PasswordHelper.HashPassword(request.Password);

                var createItem = new Account();
                if (request.RoleId != (int)EnumType.Role.Lessor && request.RoleId != (int)EnumType.Role.Lesses)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Khong dung ROLE");
                }

                var isEmail = IsEmail(request.Username);
                if (isEmail)
                {
                    createItem = new Account()
                    {
                        AccountId = Guid.NewGuid().ToString("N"),
                        Email = request.Username,
                        FullName = request.FullName,
                        PasswordHash = hashedPassword,
                        PhoneNumber = request.PhoneNumber,
                        DateOfBirth = null,
                        GenderId = null,
                        OldId = null,
                        AvatarUrl = null,
                        RoleId = request.RoleId,
                        PublicPostTimes = 0,
                        IsVerify = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = null,
                        DeletedAt = null,
                        Status = true,
                    };
                }
                else
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Username không đúng chuẩn");
                }

                if(request.RoleId == 2)
                {
                    createItem.PublicPostTimes = 1;
                }

                var result = await _unitOfWork.AccountRepository.CreateAsync(createItem);
                if (result > 0)
                {
                    var resposeData = createItem;
                    return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo tài khoản thành công", resposeData);
                }

                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tạo tài khoản thất bại");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(string id, PutAccountReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var item = await _unitOfWork.AccountRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.AccountId && roleId != ((int)EnumType.Role.Admin).ToString())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (!(bool)item.Status!)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Status == False.");
            }

            item.FullName = request.FullName;
            item.DateOfBirth = request.DateOfBirth;
            item.OldId = request.OldId;
            item.GenderId = request.GenderId;
            item.AvatarUrl = request.AvatarUrl;
            item.UpdatedAt = DateTime.Now;


            var result = await _unitOfWork.AccountRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Cập nhật thất bại");
        }
        public async Task<ServiceResult> UpdatePassword(string id, string newPassword)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var item = await _unitOfWork.AccountRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.AccountId && roleId != ((int)EnumType.Role.Admin).ToString())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (!(bool)item.Status!)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Status == False.");
            }

            item.PasswordHash = newPassword;
            item.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.AccountRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Cập nhật thất bại");
        }
        public async Task<ServiceResult> UpdateStatus(string id)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            var item = await _unitOfWork.AccountRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.AccountId && roleId != ((int)EnumType.Role.Admin).ToString())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            item.DeletedAt = DateTime.Now;
            item.Status = false;

            var result = await _unitOfWork.AccountRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Xóa mềm thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
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
        private bool IsEmail(string input)
        {
            return input.Contains("@");
        }
        private bool IsPhoneNumber(string input)
        {
            return Regex.IsMatch(input, @"^\d{10,11}$");
        }
    }

}
