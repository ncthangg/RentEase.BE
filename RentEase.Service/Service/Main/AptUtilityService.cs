using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.Base;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{
    public interface IAptUtilityService
    {
        Task<ServiceResult> GetAll(int page, int pageSize, bool? statuss = true);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetAllForApt(string aptId, int page, int pageSize);
        Task<ServiceResult> Create(AptUtilityReq request);
        Task<ServiceResult> Update(int id, string? description);
        Task<ServiceResult> Delete(int id);

    }
    public class AptUtilityService : BaseService<AptUtility, AptUtilityRes>, IAptUtilityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptUtilityService(IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        public async Task<ServiceResult> GetAllForApt(string aptId, int page, int pageSize)
        {
            var items = await _unitOfWork.AptUtilityRepository.GetAllForAptAsync(aptId, page, pageSize);
            if (!items.Data.Any())
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }
            else
            {
                var responseData = _mapper.Map<IEnumerable<AptUtilityRes>>(items.Data);
                return new ServiceResult(Const.SUCCESS_ACTION, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(AptUtilityReq request)
        {
            var createItem = new AptUtility()
            {
                AptId = request.AptId,
                UtilityId = request.UtilityId,
                Note = request.Note,
                CreatedAt = DateTime.Now,
                UpdatedAt = null,
            };

            var result = await _unitOfWork.AptUtilityRepository.CreateAsync(createItem);

            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Tạo thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
        }
        public async Task<ServiceResult> Update(int id, string? note)
        {

            if (!await EntityExistsAsync("Id", id))
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);
            }

            var item = await _unitOfWork.AptUtilityRepository.GetByIdAsync(id);

            if (item == null)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION, "Không tồn tại");
            }

            item.Note = note;
            item.UpdatedAt = DateTime.Now;

            var result = await _unitOfWork.AptUtilityRepository.UpdateAsync(item);
            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);

        }
    }
}
