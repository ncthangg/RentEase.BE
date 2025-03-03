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
        public async Task<PagedResult<AptUtility>> GetAllForAptAsync(
                int aptId, bool? status, int page = 1, int pageSize = 10)
        {
            return await GetPagedAsync(
                filter: (o => o.AptId == aptId && (status == null || o.Status == status)),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize);
        }
    }
}
