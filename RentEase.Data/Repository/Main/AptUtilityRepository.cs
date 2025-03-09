using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{
    public class AptUtilityRepository : GenericRepository<AptUtility>
    {
        public AptUtilityRepository()
        {
        }
        public AptUtilityRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<AptUtility>> GetByAptId(
                string aptId, int page = 1, int pageSize = 10)
        {
            return await GetPagedAsync(
                filter: (o => o.AptId == aptId),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize);
        }
    }
}
