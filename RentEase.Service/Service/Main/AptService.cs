using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IAptService
    {
        Task<ServiceResult> GetAll(bool? status, int page, int pageSize);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetByAccountId(string accountId, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(AptReq request);
        Task<ServiceResult> Update(string id, AptReq request);
        Task<ServiceResult> UpdateAptStatusId(string aptId, int aptStatusId);
        Task<ServiceResult> Deactive(string id);
        Task<ServiceResult> Delete(string id);
    }
    public class AptService : BaseService<Apt, AptRes>, IAptService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetAll(bool? status, int page, int pageSize)
        {
            var items = await _unitOfWork.AptRepository.GetAll(status, page, pageSize);

            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<AptRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> GetByAccountId(string accountId, bool? status, int page, int pageSize)
        {
            var items = await _unitOfWork.AptRepository.GetByAccountId(accountId, page, pageSize, status);

            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<AptRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(AptReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var category = await _unitOfWork.AptCategoryRepository.GetByIdAsync(request.AptCategoryId);

            var aptId = await GenerateUniqueAptIdAsync(category.CategoryName);

            var createItem = new Apt();
            if (string.IsNullOrEmpty(request.OwnerName) && string.IsNullOrEmpty(request.OwnerPhone) && string.IsNullOrEmpty(request.OwnerEmail))
            {
                var accountInfo = await _unitOfWork.AccountRepository.GetByIdAsync(accountId);

                createItem = new Apt()
                {
                    AptId = aptId,
                    PosterId = accountId,
                    OwnerName = accountInfo.FullName,
                    OwnerPhone = accountInfo.PhoneNumber,
                    OwnerEmail = accountInfo.Email,
                    Name = request.Name,
                    Area = request.Area,
                    Address = request.Address,
                    ProvinceId = request.ProvinceId,
                    DistrictId = request.DistrictId,
                    WardId = request.WardId,
                    AddressLink = request.AddressLink,
                    AptCategoryId = request.AptCategoryId,
                    AptStatusId = (int)EnumType.AptStatusId.AVAILABLE,
                    NumberOfRoom = request.NumberOfRoom,
                    NumberOfSlot = request.NumberOfSlot,
                    Note = request.Note,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    DeletedAt = null,
                    Status = false,
                };
            }
            else
            {
                createItem = new Apt()
                {
                    AptId = aptId,
                    PosterId = accountId,
                    OwnerName = request.OwnerName,
                    OwnerPhone = request.OwnerPhone,
                    OwnerEmail = request.OwnerEmail,
                    Name = request.Name,
                    Area = request.Area,
                    Address = request.Address,
                    ProvinceId = request.ProvinceId,
                    DistrictId = request.DistrictId,
                    WardId = request.WardId,
                    AddressLink = request.AddressLink,
                    AptCategoryId = request.AptCategoryId,
                    AptStatusId = (int)EnumType.AptStatusId.AVAILABLE,
                    NumberOfRoom = request.NumberOfRoom,
                    NumberOfSlot = request.NumberOfSlot,
                    Note = request.Note,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    DeletedAt = null,
                    Status = false,
                };
            }

            var result = await _unitOfWork.AptRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(string id, AptReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var item = await _unitOfWork.AptRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.PosterId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            var updateItem = new Apt()
            {
                AptId = item.AptId,
                PosterId = item.PosterId,
                OwnerName = item.OwnerName,
                OwnerPhone = item.OwnerPhone,
                OwnerEmail = item.OwnerEmail,
                Name = request.Name,
                Area = request.Area,
                Address = request.Address,
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                WardId = request.WardId,
                AddressLink = request.AddressLink,
                AptCategoryId = item.AptCategoryId,
                AptStatusId = item.AptStatusId,
                NumberOfRoom = request.NumberOfRoom,
                NumberOfSlot = request.NumberOfSlot,
                Note = request.Note,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = null,
                Status = item.Status,
            };

            var result = await _unitOfWork.AptRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> UpdateAptStatusId(string aptId, int aptStatusId)
        {
            if (aptStatusId != (int)EnumType.AptStatusId.AVAILABLE &&
                     aptStatusId != (int)EnumType.AptStatusId.UNAVAILABLE )
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "AptStatusId không hợp lệ.");
            }

            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var item = await _unitOfWork.AptRepository.GetByIdAsync(aptId);

            if (accountId != item.PosterId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (!await EntityExistsAsync("AptId", aptId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Apt không tồn tại");
            }

            item.AptStatusId = aptStatusId;
            item.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.AptRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật AptStatus thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Cập nhật AptStatus thất bại");
        }
        public async Task<ServiceResult> Deactive(string id)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            var item = await _unitOfWork.AptRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.PosterId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            item.UpdatedAt = DateTime.Now;
            item.Status = false;

            var result = await _unitOfWork.AptRepository.UpdateAsync(item);

            if (result > 0)
            {
                var listPost = _unitOfWork.PostRepository.GetByAptId(item.AptId);
                foreach (var post in listPost.Result)
                {
                    post.Status = false;
                    post.UpdatedAt = DateTime.Now;
                    await _unitOfWork.PostRepository.UpdateAsync(post);
                }
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Private");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }


        private async Task<string> GenerateUniqueAptIdAsync(string categoryName)
        {
            string aptId;
            bool isDuplicate;

            do
            {
                // Tạo mã aptCode mới
                aptId = this.GenerateAptId(categoryName);

                // Kiểm tra xem mã đã tồn tại trong DB chưa
                isDuplicate = await EntityExistsAsync("AptId", aptId);

            } while (isDuplicate);

            return aptId;
        }
        private string GenerateAptId(string categoryName)
        {
            string prefix = string.Concat(categoryName.Split(' '))
                                  .ToUpper()
                                  .Substring(0, Math.Min(3, categoryName.Length));

            Random random = new Random();
            int randomNumber = random.Next(100000, 999999);  // 6 số ngẫu nhiên

            return $"{prefix}{randomNumber}";
        }


    }
}
