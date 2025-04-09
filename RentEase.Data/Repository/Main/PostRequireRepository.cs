using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{
    public class PostRequireRepository : GenericRepository<PostRequire>
    {
        public PostRequireRepository()
        {
        }
        public PostRequireRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<PostRequire>> GetByAccountIdAsync(
                  string accountId, int? approveStatusId, int page = 1, int pageSize = 10)
        {
            return await GetPagedAsync(
                filter: o => o.AccountId == accountId &&
                          (approveStatusId == null || o.ApproveStatusId == approveStatusId),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize);
        }
        public async Task<PagedResult<PostRequire>> GetByPostIdAsync(
          string postId, int page = 1, int pageSize = 10)
        {
            return await GetPagedAsync(
                filter: o => o.PostId == postId,
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize);
        }
        public async Task<PostRequire?> GetByPostIdAndAccountIdAsync(string postId, string accountId)
        {
            return await _context.Set<PostRequire>()
                .Where(p => p.PostId == postId && p.AccountId == accountId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
        }


        //public async Task<PagedResult<Account>> GetBySearchAsync(string? fullName, string? email, string? phoneNumber, int page, int pageSize)
        //{
        //    IQueryable<Account> query = _context.Set<Account>();

        //    Expression<Func<Account, bool>> filter = a =>
        //        (string.IsNullOrEmpty(fullName) || a.FullName.Contains(fullName)) &&
        //        (string.IsNullOrEmpty(email) || a.Email.Contains(email)) &&
        //        (string.IsNullOrEmpty(phoneNumber) || a.PhoneNumber.Contains(phoneNumber));

        //    return await GetPagedAsync(filter, null, page, pageSize);
        //}
    }
}
