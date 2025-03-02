using RentEase.Common.DTOs.Response;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Sub
{
    public class AptStatusRepository : GenericRepository<AptStatus>
    {
        public AptStatusRepository()
        {
        }
        public AptStatusRepository(RentEaseContext context) => _context = context;

        public async Task<PagedResult<AptStatus>> GetAllAsync(bool? status, int page, int pageSize)
        {
            IQueryable<AptStatus> query = _context.Set<AptStatus>();

            Expression<Func<AptStatus, bool>> filter = a =>
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }

        public async Task<PagedResult<AptStatus>> GetBySearchAsync(string? statusName, bool? status, int page, int pageSize)
        {
            IQueryable<AptStatus> query = _context.Set<AptStatus>();

            Expression<Func<AptStatus, bool>> filter = a =>
                (string.IsNullOrEmpty(statusName) || a.StatusName.Contains(statusName)) &&
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }
    }
}
