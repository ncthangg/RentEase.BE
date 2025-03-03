using AutoMapper;
using Azure;
using MailKit.Search;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;
using static RentEase.Common.Base.EnumType;

namespace RentEase.Service.Service.Main
{
    public interface IAptService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize, bool? status);
        Task<ServiceResult> GetByIdAsync(int id); 
        Task<ServiceResult> GetAllForAccountAsync(bool status, int page, int pageSize);
        Task<ServiceResult> Create(RequestAptDto request);
        Task<ServiceResult> Update(int id, RequestAptDto request, int? aptStatus, int? approveStatus);
        Task<ServiceResult> Delete(int id);

    }
    public class AptService : BaseService<Apt, ResponseAptDto>, IAptService
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
        public async Task<ServiceResult> GetAllForAccountAsync(bool status,int page, int pageSize)
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

            var accountRole = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountRole != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.AptRepository.GetAllForAccountAsync(accountIdInt, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseAptDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(RequestAptDto request)
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

            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Name đã tồn tại");
            }

            var category = await _unitOfWork.AptCategoryRepository.GetByIdAsync(request.CategoryId);

            var aptCode = await GenerateUniqueAptCodeAsync(category.CategoryName);
            
            var createItem = new Apt()
            {
                OwnerId = accountIdInt,
                AptCode = aptCode,
                Name = request.Name,
                Description = request.Description,
                Area = request.Area,
                AddressLink = request.AddressLink,
                Address = request.Address,
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                WardId = request.WardId,
                RentPrice = request.RentPrice,
                PilePrice = request.PilePrice,
                CategoryId = request.CategoryId,
                StatusId = 1,
                NumberOfRoom = request.NumberOfRoom,
                AvailableRoom = request.AvailableRoom,
                ApproveStatusId = (int)EnumType.ApproveStatus.Pending,
                Note = request.Note,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.AptRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(int id, RequestAptDto request, int? aptStatus, int? approveStatus)
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

            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = _mapper.Map<Apt>((ResponseAptDto)(await GetByIdAsync(id)).Data);

            if (approveStatus != (int)EnumType.ApproveStatus.Pending &&
                    approveStatus != (int)EnumType.ApproveStatus.Approved &&
                        approveStatus != (int)EnumType.ApproveStatus.Rejected)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "ApproveStatus không hợp lệ.");
            }

            if (aptStatus != 1 && aptStatus != 2 && aptStatus != 3 && aptStatus != 4 && aptStatus != 5)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "AptStatus không hợp lệ.");
            }

            if (accountIdInt == 1)
            {
                item.StatusId = (int)aptStatus;
                item.ApproveStatusId = (int)approveStatus;
                item.UpdatedAt = DateTime.Now;

                var result1 = await _unitOfWork.AptRepository.UpdateAsync(item);
                if (result1 > 0)
                {
                    var responseData = _mapper.Map<ResponseAptDto>(item);

                    return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
                }
            }

            var updateItem = new Apt()
            {
                Id = id,
                OwnerId = item.OwnerId,
                AptCode = item.AptCode,
                Name = request.Name,
                Description = request.Description,
                Area = request.Area,
                AddressLink = request.AddressLink,
                Address = request.Address,
                ProvinceId = request.ProvinceId,
                DistrictId = request.DistrictId,
                WardId = request.WardId,
                RentPrice = request.RentPrice,
                PilePrice = request.PilePrice,
                CategoryId = item.CategoryId,
                StatusId = (int)aptStatus,
                NumberOfRoom = request.NumberOfRoom,
                AvailableRoom = request.AvailableRoom,
                ApproveStatusId = (int)approveStatus,
                Note = request.Note,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = null,
                Status = item.Status,
            };

            var result = await _unitOfWork.AptRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptDto>(updateItem);

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
            var item = (Apt)(await GetByIdAsync(id)).Data;

            if (item != null)
            {
                item.DeletedAt = DateTime.Now;
                item.Status = false;
            }

            var result = await _unitOfWork.AptRepository.UpdateAsync(item);

            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseAptDto>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        private async Task<string> GenerateUniqueAptCodeAsync(string categoryName)
        {
            string aptCode;
            bool isDuplicate;

            do
            {
                // Tạo mã aptCode mới
                aptCode = _helperWrapper.TokenHelper.GenerateAptCode(categoryName);

                // Kiểm tra xem mã đã tồn tại trong DB chưa
                isDuplicate = await EntityExistsAsync("AptCode", aptCode);

            } while (isDuplicate);

            return aptCode;
        }


    }
}
