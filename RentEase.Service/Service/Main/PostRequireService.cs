using AutoMapper;
using Microsoft.AspNetCore.Http;
using RentEase.Common.DTOs.Dto;
using RentEase.Data;
using RentEase.Data.Models;
using RentEase.Service.Service.Base;

namespace RentEase.Service.Service.Main
{

    public interface IPostRequireService
    {
        //Task<ServiceResult> GetAll(int page, int pageSize, bool? status);
        //Task<ServiceResult> GetById(string id);
        //Task<ServiceResult> GetByAccountId(string accountId, int page, int pageSize);
        //Task<ServiceResult> Create(PostRequireReq request);
        //Task<ServiceResult> Update(string orderId, int? newStatus);
        //Task<ServiceResult> Delete(string id);

    }
    public class PostRequireService : BaseService<PostRequire, PostRequireRes>, IPostRequireService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HelperWrapper _helperWrapper;
        public PostRequireService(IHttpContextAccessor httpContextAccessor, IMapper mapper, HelperWrapper helperWrapper)
        : base(mapper)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
            _helperWrapper = helperWrapper;
        }
        //public async Task<ServiceResult> Create(PostRequireReq request)
        //{
        //    var orderType = await _unitOfWork.OrderTypeRepository.GetByIdAsync(request.OrderTypeId);
        //    var orderId = orderType.Name + DateTime.Now.ToString();
        //    var createItem = new PostRequire()
        //    {
        //        PostRequireId = Guid.NewGuid().ToString("N"),
        //        CreatedAt = DateTime.Now
        //    };

        //    var result = await _unitOfWork.PostRequireRepository.CreateAsync(createItem);
        //    if (result > 0)
        //    {
        //        return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Tạo thành công");
        //    }

        //    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        //}

        //public async Task<ServiceResult> Update(string orderId, int? newStatus)
        //{
        //    string accountId = _helperWrapper.TokenHelper.GetAccountIdFromHttpContextAccessor(_httpContextAccessor);

        //    if (string.IsNullOrEmpty(accountId))
        //    {
        //        return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy info");
        //    }


        //    if (!await EntityExistsAsync("Id", orderId))
        //    {
        //        return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        //    }

        //    var item = await _unitOfWork.PostRequireRepository.GetByIdAsync(orderId);

        //    if (newStatus != (int)EnumType.TransactionStatus.Pending &&
        //             newStatus != (int)EnumType.TransactionStatus.Success &&
        //                 newStatus != (int)EnumType.TransactionStatus.Failed)
        //    {
        //        return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "TransactionStatus không hợp lệ.");
        //    }


        //    var result = await _unitOfWork.PostRequireRepository.UpdateAsync(item);
        //    if (result > 0)
        //    {

        //        return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Cập nhật thành công");
        //    }

        //    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
        //}


    }
}
