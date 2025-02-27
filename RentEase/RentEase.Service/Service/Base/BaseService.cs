using AutoMapper;
using RentEase.Common.Base;
using RentEase.Data;

namespace RentEase.Service.Service.Base
{
    public class BaseService<T, TDto> where T : class
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BaseService(IMapper mapper)
        {
            _unitOfWork ??= new UnitOfWork();
            _mapper = mapper;
        }

        public async Task<ServiceResult> GetAllAsync(int page, int pageSize)
        {
            try
            {
                var pagedResult = await _unitOfWork.GetRepository<T>().GetPagedAsync(
                    filter: null,
                    orderBy: null,
                    page: page,
                    pageSize: pageSize
                );

                if (pagedResult.TotalCount == 0)
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, "Không có dữ liệu.");
                }

                var data = _mapper.Map<IEnumerable<TDto>>(pagedResult.Data);

                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, pagedResult.TotalCount, pagedResult.TotalPages, pagedResult.CurrentPage, data);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.FAIL_READ_CODE, "Lỗi khi lấy danh sách dữ liệu: " + ex.Message);
            }
        }

        public async Task<ServiceResult> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.GetRepository<T>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG, null);
                }

                var response = _mapper.Map<TDto>(entity);
                return new ServiceResult(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.FAIL_READ_CODE, "Lỗi khi lấy dữ liệu theo ID: " + ex.Message);
            }
        }

        public async Task<ServiceResult> DeleteByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.GetRepository<T>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult(Const.FAIL_DELETE_CODE, Const.FAIL_DELETE_MSG);
                }

                await _unitOfWork.GetRepository<T>().RemoveAsync(entity);
                var response = _mapper.Map<TDto>(entity);
                return new ServiceResult(Const.SUCCESS_DELETE_CODE, Const.SUCCESS_DELETE_MSG, response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.FAIL_DELETE_CODE, "Lỗi khi xóa dữ liệu: " + ex.Message);
            }
        }

        protected async Task<bool> EntityExistsAsync(string property, string value)
        {
            try
            {
                return await _unitOfWork.GetRepository<T>().EntityExistsByPropertyAsync(property, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi kiểm tra entity tồn tại: " + ex.Message);
                return false;
            }
        }

        protected async Task<bool> EntityExistsAsync(string property, int value)
        {
            try
            {
                return await _unitOfWork.GetRepository<T>().EntityExistsByPropertyAsync(property, value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi kiểm tra entity tồn tại: " + ex.Message);
                return false;
            }
        }


    }
}
