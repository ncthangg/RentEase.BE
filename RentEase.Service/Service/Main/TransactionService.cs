using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Common.DTOs.Response;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;
using RentEase.Service.Service.Payment;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RentEase.Service.Service.Main
{
    public interface ITransactionService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetByAccountId(string accountId, int? statusId, int page, int pageSize);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> CheckOut(TransactionReq request);
    }

    public class TransactionService : BaseService<Transaction, TransactionRes>, ITransactionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        private readonly IPayosService _payosService;
        public TransactionService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper, IPayosService payosService)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
            _payosService = payosService;
        }
        public async Task<ServiceResult> GetByAccountId(string accountId, int? statusId, int page, int pageSize)
        {
            var items = await _unitOfWork.TransactionRepository.GetByAccountId(accountId, statusId, page, pageSize);

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
        public async Task<ServiceResult> CheckOut(TransactionReq request)
        {

            string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

            if (string.IsNullOrEmpty(accountId))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tài khoản không tồn tại");
            }

            var transactionItem = await _unitOfWork.TransactionRepository.GetByOrderCode(request.OrderId);
            var orderItem = await _unitOfWork.OrderRepository.GetByIdAsync(request.OrderId);

            if (orderItem.SenderId != accountId)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Đơn hàng không thuộc tài khoản này");
            }

            //Khai báo
            TransactionRes transactionRes = new TransactionRes();
            PayosRes payosRes = new PayosRes();

            if (transactionItem != null)
            {
                // Tăng giá trị lên 1 rồi chuyển lại thành string
                var newPaymentAttempt = transactionItem.PaymentAttempt + 1;

                var createItem = new Transaction()
                {
                    TransactionTypeId = orderItem.TransactionTypeId,
                    OrderId = orderItem.OrderId,
                    PaymentAttempt = newPaymentAttempt,
                    PaymentCode = $"{orderItem.OrderId}{DateTime.Now:HHmmssfff}",
                    TotalAmount = orderItem.Amount + orderItem.IncurredCost,
                    Note = request.Note,
                    CreatedAt = DateTime.Now,
                    StatusId = (int)EnumType.StatusId.Pending
                };

                await _unitOfWork.TransactionRepository.CreateAsync(createItem);
                transactionRes = _mapper.Map<TransactionRes>(createItem);

                string jsonResponse = await _payosService.CreatePaymentURL(createItem.PaymentCode);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Bỏ qua phân biệt chữ hoa/thường
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua giá trị null
                };

                payosRes = JsonSerializer.Deserialize<PayosRes>(jsonResponse, options);

            }
            else
            {

                var createItem = new Transaction()
                {
                    TransactionTypeId = orderItem.TransactionTypeId,
                    OrderId = orderItem.OrderId,
                    PaymentAttempt = 1,
                    PaymentCode = $"{orderItem.OrderId}{DateTime.Now:HHmmssfff}",
                    TotalAmount = orderItem.Amount + orderItem.IncurredCost,
                    Note = request.Note,
                    CreatedAt = DateTime.Now,
                    StatusId = (int)EnumType.StatusId.Pending
                };

                await _unitOfWork.TransactionRepository.CreateAsync(createItem);
                transactionRes = _mapper.Map<TransactionRes>(createItem);

                string jsonResponse = await _payosService.CreatePaymentURL(createItem.PaymentCode);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true, // Bỏ qua phân biệt chữ hoa/thường
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // Bỏ qua giá trị null
                };

                payosRes = JsonSerializer.Deserialize<PayosRes>(jsonResponse, options);

            }
            if (payosRes == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Tạo link thanh toán thất bại");
            }

            var responseData = new PaymentRes()
            {
                TransactionRes = transactionRes,
                PayosRes = payosRes,
            };
            return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo link thanh toán thành công", responseData);
        }

    }
}
