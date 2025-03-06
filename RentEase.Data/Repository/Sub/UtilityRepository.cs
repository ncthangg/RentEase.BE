using RentEase.Common.DTOs;
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

            Expression<Func<Utility, bool>> filter = null;

            return await GetPagedAsync(filter, null, page, pageSize);
        }

    }
}
