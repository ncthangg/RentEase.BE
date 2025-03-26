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
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(string id);
        Task<ServiceResult> GetByOrderCode(string orderCode);
        Task<ServiceResult> GetByPaymentStatusId(int paymentStatusId, int page, int pageSize);
        Task<ServiceResult> GetByAccountId(string accountId, int? paymentStatusId, int page, int pageSize);
        Task<ServiceResult> UpdatePaymentStatusId(string orderId, int paymentStatusId); // chỉ dành cho ADMIN
        Task<ServiceResult> Delete(string id);
    }
    public class OrderService : BaseService<Order, OrderRes>, IOrderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public OrderService(IHttpContextAccessor httpContextAccesser, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccesser;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> GetByOrderCode(string orderCode)
        {

            var item = await _unitOfWork.OrderRepository.GetByOrderCodeAsync(orderCode);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<OrderRes>(item);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, responseData);
            }
        }
        public async Task<ServiceResult> GetByPaymentStatusId(int paymentStatusId, int page, int pageSize)
        {

            var items = await _unitOfWork.OrderRepository.GetByPaymentStatusId(paymentStatusId, page, pageSize);

            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<OrderRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> GetByAccountId(string accountId, int? paymentStatusId, int page, int pageSize)
        {

            var items = await _unitOfWork.OrderRepository.GetByAccountId(accountId, paymentStatusId, page, pageSize);

            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<OrderRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> UpdatePaymentStatusId(string orderId, int paymentStatusId)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("OrderId", orderId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Order không tồn tại");
            }

            var item = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

            if (accountId != item.SenderId || roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Bạn không có quyền hạn.");
            }

            if (paymentStatusId != (int)EnumType.PaymentStatusId.PENDING ||
                       paymentStatusId != (int)EnumType.PaymentStatusId.PAID ||
                            paymentStatusId != (int)EnumType.PaymentStatusId.PROCESSING ||
                                   paymentStatusId != (int)EnumType.PaymentStatusId.CANCELLED)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "ApproveStatusId không hợp lệ.");
            }

            if (paymentStatusId == (int)EnumType.PaymentStatusId.PAID)
            {
                item.PaymentStatusId = paymentStatusId;
                item.PaidAt = DateTime.Now;
            }
            else
            {
                item.PaymentStatusId = paymentStatusId;
            }

            var result = await _unitOfWork.OrderRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật Order thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Cập nhật Order thất bại");
        }



    }
}
