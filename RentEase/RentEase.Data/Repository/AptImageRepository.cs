using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository
{
    public class AptImageRepository : GenericRepository<AptImage>
    {
        public AptImageRepository()
        {
        }
        public AptImageRepository(RentEaseContext context) => _context = context;
        //public async Task<PagedResult<AptImage>> GetBySearchAsync(bool? status, int page, int pageSize)
        //{
        //    IQueryable<AptImage> query = _context.Set<AptImage>();

        //    Expression<Func<AptImage, bool>> filter = a =>
        //        (!status.HasValue || a.Status == status.Value);

        //    return await GetPagedAsync(filter, null, page, pageSize);
        //}
    }
}
