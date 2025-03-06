using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{
    public class AptImageRepository : GenericRepository<AptImage>
    {
        public AptImageRepository()
        {
        }
        public AptImageRepository(RentEaseContext context) => _context = context;

        //public async Task<PagedResult<Order>> GetOrdersForUserAsync(
        //       int userId, int page = 1, int pageSize = 10)
        //{
        //    return await GetPagedAsync(
        //        filter: o => o.UserId == userId,
        //        orderBy: q => q.OrderByDescending(o => o.CreatedAt),
        //        page: page,
        //        pageSize: pageSize);
        //}
    }
}
