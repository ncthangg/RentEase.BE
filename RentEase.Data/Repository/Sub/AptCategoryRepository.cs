using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Sub
{
    public class AptCategoryRepository : GenericRepository<AptCategory>
    {
        public AptCategoryRepository()
        {
        }
        public AptCategoryRepository(RentEaseContext context) => _context = context;

        public async Task<PagedResult<AptCategory>> GetAllAsync(bool? status, int page, int pageSize)
        {
            IQueryable<AptCategory> query = _context.Set<AptCategory>();

            Expression<Func<AptCategory, bool>> filter = null;

            return await GetPagedAsync(filter, null, page, pageSize);
        }

    }
}
