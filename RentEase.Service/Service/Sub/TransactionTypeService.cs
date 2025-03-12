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
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> Create(TransactionTypeReq request);
        Task<ServiceResult> Update(int id, TransactionTypeReq request);
        Task<ServiceResult> Delete(int id);

    }
    public class TransactionTypeService : BaseService<TransactionType, TransactionTypeRes>, ITransactionTypeService
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
        public async Task<ServiceResult> Create(TransactionTypeReq request)
        {
            if (await EntityExistsAsync("TypeName", request.TypeName))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new TransactionType()
            {
                TypeName = request.TypeName.ToLower(),
                Note = request.Note,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
            };

            var result = await _unitOfWork.TransactionTypeRepository.CreateAsync(createItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(int id, TransactionTypeReq request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.TransactionTypeRepository.GetByIdAsync(id);

            var updateItem = new TransactionType()
            {
                Id = item.Id,
                TypeName = request.TypeName.ToLower(),
                Note = request.Note,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.TransactionTypeRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        }
    }
}
