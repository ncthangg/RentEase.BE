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
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetByAptId(string aptId);
        Task<ServiceResult> Create(string aptId, List<IFormFile> files);
        Task<ServiceResult> UpdateSingleImage(int id, PostSingleAptImageReq request);
        Task<ServiceResult> DeleteSingleImage(int id);

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
        public async Task<ServiceResult> GetByAptId(string aptId)
        {
            var aptExist = await _unitOfWork.AptRepository.GetByIdAsync(aptId);
            if (aptExist == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Apt không tồn tại");
            }
            var result = await _unitOfWork.AptImageRepository.GetByAptIdAsync(aptId);
            if (result == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không có hình ảnh");
            }


            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Lấy hình ảnh theo AptId thành công", result);
        }

        public async Task<ServiceResult> Create(string aptId, List<IFormFile> files)
        {
            var aptExist = await _unitOfWork.AptRepository.GetByIdAsync(aptId);
            if (aptExist == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            List<string> uploadedUrls = new List<string>(); // Danh sách lưu URL ảnh
            var directoryPath = Path.Combine("wwwroot", "uploads", "apt");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            foreach (var file in files)
            {
                try
                {
                    var originalExt = Path.GetExtension(file.FileName).ToLower();
                    var tempFileName = $"{Guid.NewGuid()}{originalExt}"; // Lưu file tạm
                    var tempFilePath = Path.Combine(directoryPath, tempFileName);

                    // Lưu file gốc tạm thời
                    using (var stream = new FileStream(tempFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var finalFileName = $"{Guid.NewGuid()}.jpg"; // File JPG sau khi convert
                    var finalFilePath = Path.Combine(directoryPath, finalFileName);

                    if (originalExt != ".jpg" && originalExt != ".jpeg")
                    {
                        // Kiểm tra _helperWrapper.ImageHelper có null không
                        if (_helperWrapper?.ImageHelper == null)
                        {
                            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "ConvertToJpg() chưa được khởi tạo.");
                        }

                        // Convert sang JPG
                        _helperWrapper.ImageHelper.ConvertToJpg(tempFilePath, finalFilePath);
                        File.Delete(tempFilePath); // Xoá file gốc sau khi chuyển đổi
                    }
                    else
                    {
                        // Nếu là JPG sẵn, đổi tên và giữ nguyên file
                        File.Move(tempFilePath, finalFilePath);
                    }

                    var imageUrl = $"/uploads/apt/{finalFileName}";
                    uploadedUrls.Add(imageUrl);

                    // Lưu vào database
                    var createItem = new AptImage()
                    {
                        AptId = aptId,
                        ImageUrl = imageUrl,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = null,
                    };

                    await _unitOfWork.AptImageRepository.CreateAsync(createItem);
                }
                catch (Exception ex)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, $"Lỗi khi xử lý file: {ex.Message}");
                }
            }

            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công", uploadedUrls);
        }

        public async Task<ServiceResult> UpdateSingleImage(int id, PostSingleAptImageReq request)
        {
            try
            {
                string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
                string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

                if (string.IsNullOrEmpty(accountId))
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy thông tin tài khoản.");
                }

                // Lấy thông tin ảnh cần cập nhật
                var image = await _unitOfWork.AptImageRepository.GetByIdAsync(id);
                if (image == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tìm thấy ảnh.");
                }

                // Lấy thông tin căn hộ của ảnh
                var apt = await _unitOfWork.AptRepository.GetByIdAsync(image.AptId);
                if (apt == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Căn hộ không tồn tại.");
                }

                // Kiểm tra quyền sở hữu hoặc quyền admin
                if (accountId != apt.OwnerId && roleId != "1")
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền chỉnh sửa.");
                }

                if (id != image.Id || request.AptId != image.AptId)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Id và AptId không khớp");
                }

                // Thư mục lưu ảnh
                var uploadFolder = Path.Combine("wwwroot", "uploads", "apt");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Xóa file ảnh cũ nếu tồn tại
                if (!string.IsNullOrEmpty(image.ImageUrl))
                {
                    var oldImagePath = Path.Combine("wwwroot", image.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Xử lý định dạng file
                var originalExt = Path.GetExtension(request.File.FileName).ToLower();
                var tempFileName = $"{Guid.NewGuid()}{originalExt}"; // Lưu file tạm
                var tempFilePath = Path.Combine(uploadFolder, tempFileName);

                // Lưu file gốc tạm thời
                using (var stream = new FileStream(tempFilePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(stream);
                }

                var finalFileName = $"{Guid.NewGuid()}.jpg"; // File JPG sau khi convert
                var finalFilePath = Path.Combine(uploadFolder, finalFileName);

                if (originalExt != ".jpg" && originalExt != ".jpeg")
                {
                    // Kiểm tra nếu ConvertToJpg có sẵn
                    if (_helperWrapper?.ImageHelper == null)
                    {
                        return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "ConvertToJpg() chưa được khởi tạo.");
                    }

                    // Convert sang JPG
                    _helperWrapper.ImageHelper.ConvertToJpg(tempFilePath, finalFilePath);
                    File.Delete(tempFilePath); // Xóa file gốc sau khi chuyển đổi
                }
                else
                {
                    // Nếu là JPG sẵn, đổi tên và giữ nguyên file
                    File.Move(tempFilePath, finalFilePath);
                }

                string newImageUrl = $"/uploads/apt/{finalFileName}";

                // Cập nhật đường dẫn ảnh trong database
                image.ImageUrl = newImageUrl;
                image.UpdatedAt = DateTime.UtcNow;

                var result = await _unitOfWork.AptImageRepository.UpdateAsync(image);
                if (result > 0)
                {
                    return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật ảnh thành công", newImageUrl);
                }

                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Cập nhật ảnh thất bại.");
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, $"Lỗi khi cập nhật ảnh: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteSingleImage(int id)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy thông tin tài khoản.");
            }

            // Lấy thông tin ảnh từ database
            var image = await _unitOfWork.AptImageRepository.GetByIdAsync(id);
            if (image == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không tìm thấy ảnh.");
            }

            // Lấy thông tin căn hộ của ảnh
            var apt = await _unitOfWork.AptRepository.GetByIdAsync(image.AptId);
            if (apt == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Căn hộ không tồn tại.");
            }

            // Kiểm tra quyền sở hữu hoặc quyền admin
            if (accountId != apt.OwnerId && roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền xóa ảnh này.");
            }

            // Xóa file ảnh khỏi thư mục
            var filePath = Path.Combine("wwwroot", image.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Xóa dữ liệu ảnh khỏi database
            var result = await _unitOfWork.AptImageRepository.RemoveAsync(image);
            if (result)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Xóa ảnh thành công.");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Xóa ảnh thất bại.");
        }


    }
}
