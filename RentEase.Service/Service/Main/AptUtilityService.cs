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
        Task<ServiceResult> GetAll(int page, int pageSize, bool? statuss);
        Task<ServiceResult> GetById(int id);
        Task<ServiceResult> GetAllForApt(string aptId, int page, int pageSize);
        Task<ServiceResult> Create(string aptId, int utilityId, string? note);
        Task<ServiceResult> Update(int id, string? note);
        Task<ServiceResult> Delete(int id);

    }
    public class AptUtilityService : BaseService<AptUtility, AptUtilityRes>, IAptUtilityService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public AptUtilityService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
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
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, items.TotalCount, items.TotalPages, items.CurrentPage, responseData);
            }
        }
        public async Task<ServiceResult> Create(string aptId, int utilityId, string? note)
        {
            var item = new AptUtility()
            {
                AptId = aptId,
                UtilityId = utilityId,
                Note = note,
                CreatedAt = DateTime.Now,
            };

            var result = await _unitOfWork.AptUtilityRepository.CreateAsync(item);

            if (result > 0)
            {
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
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
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
            }

            return new ServiceResult(Const.ERROR_EXCEPTION, Const.ERROR_EXCEPTION_MSG);

        }
    }
}
