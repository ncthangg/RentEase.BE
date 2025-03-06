using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IOrderService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status = true);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetAllOwn(int? statusId, int page, int pageSize);
        Task<ServiceResult> Create(OrderReq request);
        Task<ServiceResult> Delete(string id);

    }
    public class OrderService : BaseService<Order, OrderRes>, IOrderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public OrderService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetAllOwn(int? statusId, int page, int pageSize)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            var items = await _unitOfWork.OrderRepository.GetAllOwn(accountId, statusId, page, pageSize);

            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<OrderRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(OrderReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            var transactionType = await _unitOfWork.TransactionTypeRepository.GetByIdAsync(request.TransactionTypeId);
            var createItem = new Order()
            {
                OrderId = transactionType.TypeName + Guid.NewGuid().ToString("N"),
                SenderId = accountId,
                Amount = request.Amount,
                IncurredCost = request.IncurredCost,
                CreatedAt = DateTime.Now
            };

            var result = await _unitOfWork.OrderRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var resultData = _mapper.Map<OrderRes>(createItem);
                return new ServiceResult(Const.SUCCESS_ACTION, "Tạo thành công", resultData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }


    }
}
