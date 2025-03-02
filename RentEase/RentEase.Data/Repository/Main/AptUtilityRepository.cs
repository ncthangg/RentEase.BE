using RentEase.Common.DTOs.Response;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Main
{
    public class AptUtilityRepository : GenericRepository<AptUtility>
    {
        public AptUtilityRepository()
        {
        }
        public AptUtilityRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<AptUtility>> GetAllAsync(bool? status, int page, int pageSize)
        {
            IQueryable<AptUtility> query = _context.Set<AptUtility>();

            Expression<Func<AptUtility, bool>> filter = a =>
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }
    }
}
