using RentEase.Common.DTOs.Response;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Sub
{
    public class UtilityRepository : GenericRepository<Utility>
    {
        public UtilityRepository()
        {
        }
        public UtilityRepository(RentEaseContext context) => _context = context;

        public async Task<PagedResult<Utility>> GetAllAsync(bool? status, int page, int pageSize)
        {
            IQueryable<Utility> query = _context.Set<Utility>();

            Expression<Func<Utility, bool>> filter = a =>
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }

        public async Task<PagedResult<Utility>> GetBySearchAsync(string? utilityName, bool? status, int page, int pageSize)
        {
            IQueryable<Utility> query = _context.Set<Utility>();

            Expression<Func<Utility, bool>> filter = a =>
                (string.IsNullOrEmpty(utilityName) || a.UtilityName.Contains(utilityName)) &&
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }
    }
}
