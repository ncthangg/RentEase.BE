using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Sub
{
    public interface IAptStatusService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> Create(AptStatusReq request);
        Task<ServiceResult> Update(int id, AptStatusReq request);
        Task<ServiceResult> Delete(int id);

    }
    public class AptStatusService : BaseService<AptStatus, AptStatusRes>, IAptStatusService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptStatusService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }

        public async Task<ServiceResult> Create(AptStatusReq request)
        {
            if (await EntityExistsAsync("StatusName", request.StatusName))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var createItem = new AptStatus()
            {
                StatusName = request.StatusName.ToLower(),
                Note = request.Note,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
            };

            var result = await _unitOfWork.AptStatusRepository.CreateAsync(createItem);
            if (result > 0)
            {

                return new ServiceResult(Const.SUCCESS_ACTION, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(int id, AptStatusReq request)
        {
            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.AptStatusRepository.GetByIdAsync(id);

            var updateItem = new AptStatus()
            {
                Id = item.Id,
                StatusName = request.StatusName.ToLower(),
                Note = request.Note,
                CreatedAt = item.CreatedAt,
                UpdatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.AptStatusRepository.UpdateAsync(updateItem);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }

    }
}
