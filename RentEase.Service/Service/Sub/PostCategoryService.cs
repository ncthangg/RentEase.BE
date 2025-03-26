using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Sub
{

    public interface IPostCategoryService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> Create(PostCategoryReq request);
        Task<ServiceResult> Update(int id, PostCategoryReq request);
        Task<ServiceResult> Delete(int id);

    }
    public class PostCategoryService : BaseService<PostCategory, PostCategoryRes>, IPostCategoryService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public PostCategoryService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> Create(PostCategoryReq request)
        {
            if (await EntityExistsAsync("CategoryName", request.CategoryName))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new PostCategory()
            {
                CategoryName = request.CategoryName.ToLower(),
                Note = request.Note,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
            };

            var result = await _unitOfWork.PostCategoryRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(int id, PostCategoryReq request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.PostCategoryRepository.GetByIdAsync(id);

            var updateItem = new PostCategory()
            {
                Id = item.Id,
                CategoryName = request.CategoryName.ToLower(),
                Note = request.Note,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now
            };

            var result = await _unitOfWork.PostCategoryRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
