using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Collections.Generic;

namespace RentEase.Data.Repository.Main
{

    public class PostRepository : GenericRepository<Post>
    {
        public PostRepository()
        {
        }
        public PostRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<Post>> GetAll(bool? status, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: (f => (!status.HasValue || f.Status == status.Value)),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q
                         .Include(i => i.Apt)
            );
        }
        public async Task<PagedResult<Post>> GetByAccountId(
                  string accountId, bool? status, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: o => o.PosterId == accountId &&
                             (status == null || o.Status == status),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize);
        }
        public async Task<Post?> GetByAccountId(
                  string accountId, bool? status)
        {
            return await _context.Set<Post>()
                .Where(p => p.PosterId == accountId && p.Status == status)
                .FirstOrDefaultAsync();
        }
        public async Task<Post?> GetByAccountIdAndAptIdAsync(string accountId, string aptId)
        {
            return await _context.Set<Post>()
                .Where(p => p.PosterId == accountId && p.AptId == aptId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
        }
        public async Task<List<Post>> GetByAptId(string aptId)
        {
            return await _context.Set<Post>()
                .Where(p => p.AptId == aptId)
                .ToListAsync();
        }
        public async Task<List<Post>> GetByAccountId(string account)
        {
            return await _context.Set<Post>()
                .Where(p => p.PostId == account)
                .ToListAsync();
        }

    }
}
