using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{

    public class PostRepository : GenericRepository<Post>
    {
        public PostRepository()
        {
        }
        public PostRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<Post>> GetAll(int? approveStatusId, bool? status, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: (f => (!approveStatusId.HasValue || f.ApproveStatusId == approveStatusId) &&
                                (!status.HasValue || f.Status == status.Value)),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q
                         .Include(i => i.Apt)
            );
        }
        public async Task<PagedResult<Post>> GetByAccountId(
                  string accountId, int? approveStatusId, bool? status, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: o => o.AccountId == accountId &&
                             (approveStatusId == null || o.ApproveStatusId == approveStatusId) &&
                             (status == null || o.Status == status),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize);
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
