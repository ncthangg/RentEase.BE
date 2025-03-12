using AutoMapper;
using RentEase.Common.Base;
using RentEase.Data;
using System.Linq.Expressions;
using System.Reflection;

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

        public async Task<ServiceResult> GetAll(int page, int pageSize, bool? status)
        {
            try
            {
                Expression<Func<T, bool>>? filter = null;
                var entityType = typeof(T);
                var statusProperty = entityType.GetProperty("Status", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                if (status.HasValue && statusProperty != null && 
                    (statusProperty.PropertyType == typeof(bool) || statusProperty.PropertyType == typeof(bool?)))
                {
                    var parameter = Expression.Parameter(typeof(T), "o");
                    var property = Expression.Property(parameter, statusProperty);
                    if (statusProperty.PropertyType == typeof(bool?))
                    {
                        // Kiểm tra nếu có giá trị (o.Status.HasValue && o.Status.Value == status.Value)
                        var hasValueCheck = Expression.Property(property, "HasValue");
                        var valueAccess = Expression.Property(property, "Value");

                        var statusValue = Expression.Constant(status.Value, typeof(bool));

                        var hasValueExpression = Expression.Equal(hasValueCheck, Expression.Constant(true));
                        var equalExpression = Expression.Equal(valueAccess, statusValue);

                        var finalExpression = Expression.AndAlso(hasValueExpression, equalExpression);

                        filter = Expression.Lambda<Func<T, bool>>(finalExpression, parameter);
                    }
                    else
                    {
                        var equalExpression = Expression.Equal(property, Expression.Constant(status.Value));
                        filter = Expression.Lambda<Func<T, bool>>(equalExpression, parameter);
                    }
                }

                var pagedResult = await _unitOfWork.GetRepository<T>().GetPagedAsync(
                    filter: filter,
                    orderBy: null,
                    page: page,
                    pageSize: pageSize
                );

                if (pagedResult.TotalCount == 0)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Không có dữ liệu.");
                }

                var data = _mapper.Map<IEnumerable<TDto>>(pagedResult.Data);

                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, pagedResult.TotalCount, pagedResult.TotalPages, pagedResult.CurrentPage, data);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy danh sách: " + ex.Message);
            }
        }

        public async Task<ServiceResult> GetById(int id)
        {
            try
            {
                var entity = await _unitOfWork.GetRepository<T>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
                }

                var response = _mapper.Map<TDto>(entity);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy dữ liệu theo ID: " + ex.Message);
            }
        }
        public async Task<ServiceResult> GetById(string id)
        {
            try
            {
                var entity = await _unitOfWork.GetRepository<T>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
                }

                var response = _mapper.Map<TDto>(entity);
                return new ServiceResult(Const.SUCCESS_ACTION_CODE, Const.SUCCESS_ACTION_MSG, response);
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi lấy dữ liệu theo ID: " + ex.Message);
            }
        }

        public async Task<ServiceResult> Delete(int id)
        {
            try
            {
                var entity = await _unitOfWork.GetRepository<T>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
                }

                var result = await _unitOfWork.GetRepository<T>().RemoveAsync(entity);
                if (result)
                {
                    return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Xóa thành công");

                }
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Xóa thất bại");
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi xóa: " + ex.Message);
            }
        }
        public async Task<ServiceResult> Delete(string id)
        {
            try
            {
                var entity = await _unitOfWork.GetRepository<T>().GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult(Const.ERROR_EXCEPTION_CODE, Const.ERROR_EXCEPTION_MSG);
                }

                var result = await _unitOfWork.GetRepository<T>().RemoveAsync(entity);
                if (result)
                {
                    return new ServiceResult(Const.SUCCESS_ACTION_CODE, "Xóa thành công");

                }
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Xóa thất bại");
            }
            catch (Exception ex)
            {
                return new ServiceResult(Const.ERROR_EXCEPTION_CODE, "Lỗi khi xóa: " + ex.Message);
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
