using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service
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
        Task<ServiceResult> Update(int id, RequestAccountDto request);
        Task<ServiceResult> Delete(int id);
        Task<bool> AccountExist(int id);
        Task<bool> AccountExistByMail(string email);
        Task<bool> AccountExistByPhoneNumber(string phoneNumber);
        bool IsEmail(string input);

    }
    public class AccountService : BaseService<Account, ResponseAccountDto>, IAccountService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AccountService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetByEmailAsync(string email)
        {
            try
            {
                var item = await _unitOfWork.AccountRepository.GetByEmailAsync(email);
                if (item == null)
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                var responseData = _mapper.Map<ResponseAccountDto>(item);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.FAIL_READ_CODE, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetByPhoneAsync(string phoneNumber)
        {
            try
            {
                var item = await _unitOfWork.AccountRepository.GetByPhoneAsync(phoneNumber);
                if (item == null)
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                var responseData = _mapper.Map<ResponseAccountDto>(item);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.FAIL_READ_CODE, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetByEmailOrPhoneAsync(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                var item = await _unitOfWork.AccountRepository.GetByEmailOrPhoneAsync(username);
                if (item != null)
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                var responseData = _mapper.Map<ResponseAccountDto>(item);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, responseData);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.FAIL_READ_CODE, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
            }
        }

        //public async Task<ServiceResult> GetByEmailOrPhoneAsync(string username)
        //{
        //    try
        //    {
        //        var isEmail = this.IsEmail(username);
        //        var item = new Account();
        //        if (isEmail)
        //        {
        //            item = await _unitOfWork.AccountRepository.GetByEmailAsync(username);
        //        }
        //        else
        //        {
        //            item = await _unitOfWork.AccountRepository.GetByPhoneAsync(username);
        //        }

        //        if (item == null)
        //        {
        //            return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, null);
        //        }

        //        var responseData = _mapper.Map<ResponseAccountDto>(item);
        //        return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, responseData);
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ServiceResult(Const.FAIL_READ_CODE, "Lỗi khi lấy dữ liệu theo Email: " + ex.Message);
        //    }
        //}

        public async Task<ServiceResult> Search(string? fullName, string? email, string? phoneNumber, bool? isActive, bool? status, int page, int pageSize)
        {
            var items = await _unitOfWork.AccountRepository.GetBySearchAsync(fullName, email, phoneNumber, isActive, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAccountDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }

        public async Task<ServiceResult> Create(RequestAccountDto request)
        {
            if (await this.AccountExistByMail(request.Email) || await this.AccountExistByPhoneNumber(request.PhoneNumber))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
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
                IsActive = request.IsActive,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.AccountRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAccountDto>(createItem);

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestAccountDto request)
        {
            if (!await this.AccountExist(id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new Account()
            {
                Id = id,
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = request.PasswordHash,
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                AvatarUrl = request.AvatarUrl,
                RoleId = request.RoleId,
                IsActive = request.IsActive,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = null,
                Status = request.Status,
            };

            var result = await _unitOfWork.AccountRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAccountDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }

        public async Task<ServiceResult> Delete(int id)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
            }
            var item = (Account)(await this.GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.AccountRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAccountDto>(item);

                return new ServiceResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
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
