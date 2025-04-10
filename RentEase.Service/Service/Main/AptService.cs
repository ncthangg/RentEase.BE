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
        Task<ServiceResult> GetAll(int? approveStatusId, bool? status, int page, int pageSize);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetByAccountId(string accountId, int? approveStatusId, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(AptReq request);
        Task<ServiceResult> Update(string id, AptReq request);
        Task<ServiceResult> UpdateApproveStatusId(string aptId, int approveStatusId);
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
        public async Task<ServiceResult> GetAll(int? approveStatusId, bool? status, int page, int pageSize)
        {
            var items = await _unitOfWork.AptRepository.GetAll(approveStatusId, status, page, pageSize);

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
        public async Task<ServiceResult> GetByAccountId(string accountId, int? approveStatusId, bool? status, int page, int pageSize)
        {
            var items = await _unitOfWork.AptRepository.GetByAccountId(accountId, approveStatusId, page, pageSize, status);

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

            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Name đã tồn tại");
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
                    AptStatusId = request.AptStatusId,
                    NumberOfRoom = request.NumberOfRoom,
                    NumberOfSlot = request.NumberOfSlot,
                    ApproveStatusId = (int)EnumType.ApproveStatusId.Pending,
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
                    AptStatusId = request.AptStatusId,
                    NumberOfRoom = request.NumberOfRoom,
                    NumberOfSlot = request.NumberOfSlot,
                    ApproveStatusId = (int)EnumType.ApproveStatusId.Pending,
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

            if (!(bool)item.Status!)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Status == False.");
            }

            if (request.AptStatusId != (int)EnumType.AptStatusId.Full &&
                request.AptStatusId != (int)EnumType.AptStatusId.Available &&
                request.AptStatusId != (int)EnumType.AptStatusId.UnAvailable)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "AptStatus không đúng");
            }

            var updateItem = new Apt()
            {
                AptId = item.AptId,
                PosterId = item.PosterId,
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
                ApproveStatusId = item.ApproveStatusId,
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
        public async Task<ServiceResult> UpdateApproveStatusId(string aptId, int approveStatusId)
        {
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

            if (approveStatusId != (int)EnumType.ApproveStatusId.Pending &&
                       approveStatusId != (int)EnumType.ApproveStatusId.Success &&
                            approveStatusId != (int)EnumType.ApproveStatusId.Failed)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "ApproveStatusId không hợp lệ.");
            }

            if (approveStatusId == (int)EnumType.ApproveStatusId.Success)
            {
                item.ApproveStatusId = approveStatusId;
            }
            else
            {
                item.ApproveStatusId = (int)EnumType.ApproveStatusId.Failed;
            }

            item.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.AptRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật Apt thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Cập nhật Apt thất bại");
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
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Xóa mềm thành công");
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
