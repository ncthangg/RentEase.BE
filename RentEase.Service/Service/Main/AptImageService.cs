using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IAptImageService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> Create(AptImageReq request);
        Task<ServiceResult> Update(string id, AptImageReq request);
        Task<ServiceResult> Delete(string id);

    }
    public class AptImageService : BaseService<AptImage, AptImageRes>, IAptImageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptImageService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(AptImageReq request)
        {
            if (await EntityExistsAsync("AptId", request.AptId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new AptImage()
            {
                AptId = request.AptId,
                ImageUrl1 = request.ImageUrl1,
                ImageUrl2 = request.ImageUrl2,
                ImageUrl3 = request.ImageUrl3,
                ImageUrl4 = request.ImageUrl4,
                ImageUrl5 = request.ImageUrl5,
                ImageUrl6 = request.ImageUrl6,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
            };

            var result = await _unitOfWork.AptImageRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(string id, AptImageReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("AptId", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Không tồn tại");
            }

            var item = await _unitOfWork.AptImageRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Không tồn tại");
            }

            if (accountId != item.Apt.OwnerId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Bạn không có quyền hạn.");
            }

            if (!(bool)item.Apt.Status!)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Status == False.");
            }

            var updateItem = new AptImage()
            {
                AptId = item.AptId,
                ImageUrl1 = request.ImageUrl1,
                ImageUrl2 = request.ImageUrl2,
                ImageUrl3 = request.ImageUrl3,
                ImageUrl4 = request.ImageUrl4,
                ImageUrl5 = request.ImageUrl5,
                ImageUrl6 = request.ImageUrl6,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.AptImageRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

    }
}
