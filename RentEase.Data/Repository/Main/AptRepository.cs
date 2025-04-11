using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{

    public class AptRepository : GenericRepository<Apt>
    {
        public AptRepository()
        {
        }
        public AptRepository(RentEaseContext context) => _context = context;

        public async Task<PagedResult<Apt>> GetAll(bool? status, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: (f => (!status.HasValue || f.Status == status.Value)),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q
                         .Include(i => i.AptCategory)
            );
        }
        public async Task<PagedResult<Apt>> GetByAccountId(
          string accountId, int page, int pageSize, bool? status)
        {
            return await GetPagedAsync(
            filter: (f => f.PosterId == accountId &&
                        (!status.HasValue || f.Status == status.Value)),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q
                         .Include(i => i.AptCategory)
            );
        }

    }
}
