using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface ITransactionService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status = true);
        Task<ServiceResult> GetById(int id);
        Task<PaymentRes> Create(TransactionReq request);
    }

    public class TransactionService : BaseService<Transaction, TransactionRes>, ITransactionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public TransactionService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<PaymentRes> Create(TransactionReq request)
        {
            string paymentLink = "";

            string accountId = _helperWrapper.TokenHelper.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                 return new PaymentRes()
                {
                    TransactionRes = null,
                    Link = paymentLink,
                };
            }

            var transactionItem = await _unitOfWork.TransactionRepository.GetByOrderCode(request.OrderId);
            var orderItem = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);

            if (transactionItem != null)
            {
                // Tăng giá trị lên 1 rồi chuyển lại thành string
                var newPaymentAttempt = transactionItem.PaymentAttempt + 1;

                var createItem1 = new Transaction()
                {
                    TransactionTypeId = orderItem.TransactionTypeId,
                    OrderId = orderItem.OrderId,
                    PaymentAttempt = newPaymentAttempt,
                    PaymentCode = orderItem.OrderId + DateTime.Now.ToString(),
                    TotalAmount = orderItem.Amount + orderItem.IncurredCost,
                    Note = request.Note,
                    CreatedAt = DateTime.Now,
                    StatusId = (int)EnumType.StatusId.Pending
                };

                await _unitOfWork.TransactionRepository.CreateAsync(createItem1);
                var response1 = _mapper.Map<TransactionRes>(createItem1);

                //paymentLink = await _VNPayService.CreatePaymentURL(newPaymentCode, order.Amount);
                return new PaymentRes()
                {
                    TransactionRes = response1,
                    Link = paymentLink,
                };
            }

            // thanh toán lầu đầu

            var createItem2 = new Transaction()
            {
                TransactionTypeId = orderItem.TransactionTypeId,
                OrderId = orderItem.OrderId,
                PaymentAttempt = 1,
                PaymentCode = orderItem.OrderId + DateTime.Now.ToString(),
                TotalAmount = orderItem.Amount + orderItem.IncurredCost,
                Note = request.Note,
                CreatedAt = DateTime.Now,
                StatusId = (int)EnumType.StatusId.Pending
            };

            await _unitOfWork.TransactionRepository.CreateAsync(createItem2);
            var response2 = _mapper.Map<TransactionRes>(createItem2);

            //paymentLink = await _VNPayService.CreatePaymentURL(newPaymentCode, order.Amount);

            return new PaymentRes()
            {
                TransactionRes = response2,
                Link = paymentLink,
            };
        }

    }
}
