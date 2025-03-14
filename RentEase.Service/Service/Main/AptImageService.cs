using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
        Task<ServiceResult> Create(string aptId, List<IFormFile> files);
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

        public async Task<ServiceResult> Create(string aptId, List<IFormFile> files)
        {
            if (await EntityExistsAsync("AptId", aptId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            foreach (var file in files)
            {
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine("wwwroot/uploads", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = $"/uploads/{fileName}";
                //uploadedUrls.Add(imageUrl);

                // Lưu vào database
                var createItem = new AptImage()
                {
                    //AptId = aptId,
                    //ImageUrl = imageUrl,
                    //CreatedAt = DateTime.Now,
                    //UpdatedAt = null,
                };

                await _unitOfWork.AptImageRepository.CreateAsync(createItem);
            }

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
        }

        public async Task<ServiceResult> Update(string id, AptImageReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("AptId", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            var item = await _unitOfWork.AptImageRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tồn tại");
            }

            if (accountId != item.Apt.OwnerId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (!(bool)item.Apt.Status!)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Status == False.");
            }

            var updateItem = new AptImage()
            {
                //AptId = item.AptId,
                //ImageUrl = request.files,
                //CreatedAt = item.CreatedAt,
                //UpdatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.AptImageRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }

    }
}
