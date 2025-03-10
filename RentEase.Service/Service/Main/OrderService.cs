﻿using AutoMapper;
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
        Task<ServiceResult> GetByAccountId(string accountId, int? statusId, int page, int pageSize);
        Task<ServiceResult> Create(OrderReq request);
        Task<ServiceResult> Update(string orderId, int statusId); // chỉ dành cho ADMIN
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
        public async Task<ServiceResult> GetByAccountId(string accountId, int? statusId, int page, int pageSize)
        {

            var items = await _unitOfWork.OrderRepository.GetByAccountId(accountId, statusId, page, pageSize);

            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<OrderRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(OrderReq request)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }
            string orderId = Guid.NewGuid().ToString("N").Substring(0, 6);
            //var transactionType = await _unitOfWork.TransactionTypeRepository.GetByIdAsync(request.TransactionTypeId);

            var createItem = new Order()
            {
                OrderId = orderId,
                TransactionTypeId = request.TransactionTypeId,
                SenderId = accountId,
                Amount = request.Amount,
                IncurredCost = request.IncurredCost,
                CreatedAt = DateTime.Now
            };

            var result = await _unitOfWork.OrderRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var resultData = _mapper.Map<OrderRes>(createItem);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công", resultData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(string orderId, int statusId)
        {
            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);
            string roleId = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Lỗi khi lấy info");
            }

            if (!await EntityExistsAsync("OrderId", orderId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);

            if (roleId != "1")
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Bạn không có quyền hạn.");
            }

            if (statusId != (int)EnumType.StatusId.Pending &&
                     statusId != (int)EnumType.StatusId.Success &&
                         statusId != (int)EnumType.StatusId.Failed)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "StatusId không hợp lệ.");
            }

            var result = await _unitOfWork.OrderRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

    }
}
