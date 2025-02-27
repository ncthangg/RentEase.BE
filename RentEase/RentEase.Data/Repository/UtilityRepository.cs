using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository
{
    public class UtilityRepository : GenericRepository<Utility>
    {
        public UtilityRepository()
        {
        }
        public UtilityRepository(RentEaseContext context) => _context = context;
        //public async Task<PagedResult<Utility>> GetBySearchAsync(string? utilityName, bool? status, int page, int pageSize)
        //{
        //    IQueryable<Utility> query = _context.Set<Utility>();

        //    Expression<Func<Utility, bool>> filter = a =>
        //        (string.IsNullOrEmpty(utilityName) || a.Name.Contains(utilityName)) &&
        //        (!status.HasValue || a.Status == status.Value);

        //    return await GetPagedAsync(filter, null, page, pageSize);
        //}
    }
}
