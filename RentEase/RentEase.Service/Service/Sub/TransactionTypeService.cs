using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Sub
{
    public interface ITransactionTypeService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetAllAsync(bool? status, int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        Task<ServiceResult> Search(string? typeName, bool? status, int page, int pageSize);
        Task<ServiceResult> Create(RequestTransactionTypeDto request);
        Task<ServiceResult> Update(int id, RequestTransactionTypeDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class TransactionTypeService : BaseService<TransactionType, ResponseTransactionTypeDto>, ITransactionTypeService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public TransactionTypeService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetAllAsync(bool? status, int page, int pageSize)
        {
            var accountTransactionType = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountTransactionType != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.TransactionTypeRepository.GetAllAsync(status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseTransactionTypeDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Search(string? typeName, bool? status, int page, int pageSize)
        {
            var accountRole = _helperWrapper.TokenHelper.GetRoleIdFromHttpContextAccessor(_httpContextAccessor);

            if (accountRole != "1")
            {
                status = true;
            }

            var items = await _unitOfWork.TransactionTypeRepository.GetBySearchAsync(typeName, status, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<ResponseTransactionTypeDto>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(RequestTransactionTypeDto request)
        {
            if (await EntityExistsAsync("TypeName", request.TypeName))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new TransactionType()
            {
                TypeName = request.TypeName.ToLower(),
                Description = request.Description,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Status = true,
            };

            var result = await _unitOfWork.TransactionTypeRepository.CreateAsync(createItem);
            if (result > 0)
            {
                var responseData = _mapper.Map<ResponseTransactionTypeDto>(createItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestTransactionTypeDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = (TransactionType)(await GetByIdAsync(id)).Data;

            var updateItem = new TransactionType()
            {
                Id = id,
                TypeName = request.TypeName.ToLower(),
                Description = request.Description,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = item.DeletedAt,
                Status = item.Status,
            };

            var result = await _unitOfWork.TransactionTypeRepository.UpdateAsync(updateItem);
            if (result > 0)
            {

                var responseData = _mapper.Map<ResponseTransactionTypeDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, responseData);
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
