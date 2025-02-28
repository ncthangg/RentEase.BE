using AutoMapper;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service
{
    public interface ITransactionTypeService
    {
        Task<ServiceResult> GetAllAsync(int page, int pageSize);
        Task<ServiceResult> GetByIdAsync(int id);
        //Task<ServiceResult> Search(string name);
        Task<ServiceResult> Create(RequestTransactionTypeDto request);
        Task<ServiceResult> Update(int id, RequestTransactionTypeDto request);
        Task<ServiceResult> DeleteByIdAsync(int id);

    }
    public class TransactionTypeService : BaseService<TransactionType, ResponseTransactionTypeDto>, ITransactionTypeService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public TransactionTypeService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(RequestTransactionTypeDto request)
        {
            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
            }

            var createItem = new TransactionType()
            {
                Name = request.Name,
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

                return new ServiceResult(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_CREATE_CODE, Const.FAIL_CREATE_MSG);
        }

        public async Task<ServiceResult> Update(int id, RequestTransactionTypeDto request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            if (await EntityExistsAsync("Name", request.Name))
            {
                return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
            }

            var updateItem = new TransactionType()
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = request.CreatedAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = request.DeletedAt,
                Status = request.Status,
            };

            var result = await _unitOfWork.TransactionTypeRepository.UpdateAsync(updateItem);
            if (result > 0)
            {

                var responseData = _mapper.Map<ResponseTransactionTypeDto>(updateItem);

                return new ServiceResult(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, responseData);
            }

            return new ServiceResult(Const.FAIL_UPDATE_CODE, Const.FAIL_UPDATE_MSG);
        }
    }
}
