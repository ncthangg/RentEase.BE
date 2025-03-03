using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data.Models;
using RentEase.Data;
using RentEase.Service.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Search;
using Org.BouncyCastle.Asn1.X509;
using Microsoft.AspNetCore.Http;

namespace RentEase.Service.Service.Main
{

    public interface IOrderService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize, bool? status);
        Task<ServiceResult> GetByIdAsync(string id);
        Task<ServiceResult> GetAllForAccountAsync(int accountId, int page, int pageSize);
        Task<ServiceResult> Create(RequestOrderDto request);
        Task<ServiceResult> Update(string orderId, int? newStatus);
        Task<ServiceResult> DeleteByIdAsync(string id);

    }
    public class OrderService : BaseService<Order, ResponseOrderDto>, IOrderService
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
        public async Task<ServiceResult> GetAllForAccountAsync(int accountId, int page, int pageSize)
        {

            var items = await _unitOfWork.OrderRepository.GetAllForAccountAsync(accountId, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseOrderDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(RequestOrderDto request)
        {
            var transactionType = await _unitOfWork.TransactionTypeRepository.GetByIdAsync(request.TransactionTypeId);
            var orderId = transactionType.TypeName + DateTime.Now.ToString();
            var createItem = new Order()
            {
                Id = orderId,
                ContractId = request.ContractId,
                LessorId = request.LessorId,
                LesseeId = request.LesseeId,
                Amount = request.Amount,
                TransactionTypeId = request.TransactionTypeId,
                TransactionStatusId = (int)EnumType.TransactionStatus.Pending,
                DueDate = DateTime.Now.AddDays(7),
                CreatedAt = DateTime.Now
            };

            var result = await _unitOfWork.OrderRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseOrderDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(string orderId, int? newStatus)
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


            if (!await EntityExistsAsync("Id", orderId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = _mapper.Map<Order>((ResponseOrderDto)(await GetByIdAsync(orderId)).Data);

            if (newStatus != (int)EnumType.TransactionStatus.Pending &&
                     newStatus != (int)EnumType.TransactionStatus.Success &&
                         newStatus != (int)EnumType.TransactionStatus.Failed )
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "TransactionStatus không hợp lệ.");
            }

            item.TransactionStatusId = (int)newStatus;

            var result = await _unitOfWork.OrderRepository.UpdateAsync(item);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseOrderDto>(item);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }


    }
}
