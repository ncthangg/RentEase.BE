using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Sub
{
    public interface IOrderTypeService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetListByPostCategoryId(int postCategoryId);
        Task<ServiceResult> GetByPostCategoryId(int postCategoryId);
        Task<ServiceResult> Create(OrderTypeReq request);
        Task<ServiceResult> Update(int id, OrderTypeReq request);
        Task<ServiceResult> Delete(int id);

    }
    public class OrderTypeService : BaseService<OrderType, OrderTypeRes>, IOrderTypeService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public OrderTypeService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetListByPostCategoryId(int postCategoryId)
        {
            var items = await _unitOfWork.OrderTypeRepository.GetListByPostCategoryId(postCategoryId);

            if (items == null || !items.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, $"Không tìm thấy OrderType nào theo PostCategoryId = {postCategoryId}");
            }

            var responseData = _mapper.Map<IEnumerable<OrderTypeRes>>(items);
            return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, responseData);
        }
        public async Task<ServiceResult> GetByPostCategoryId(int postCategoryId)
        {
            var items = await _unitOfWork.OrderTypeRepository.GetByPostCategoryId(postCategoryId);

            if (items == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, $"Không tìm thấy OrderType nào theo PostCategoryId = {postCategoryId}");
            }

            var responseData = _mapper.Map<OrderTypeRes>(items);
            return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, responseData);
        }
        public async Task<ServiceResult> Create(OrderTypeReq request)
        {
            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new OrderType()
            {
                Id = Guid.NewGuid().ToString("N"),
                Name = request.Name.ToUpper(),
                Note = request.Note,
                Times = request.Times,
                Days = request.Days,
                Amount = request.Amount,
                PostCategoryId = request.PostCategoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
            };

            var result = await _unitOfWork.OrderTypeRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(int id, OrderTypeReq request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.OrderTypeRepository.GetByIdAsync(id);

            var updateItem = new OrderType()
            {
                Id = item.Id,
                Name = request.Name.ToUpper(),
                Note = request.Note,
                Times = request.Times,
                Days = request.Days,
                Amount = request.Amount,
                PostCategoryId = request.PostCategoryId,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.OrderTypeRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
