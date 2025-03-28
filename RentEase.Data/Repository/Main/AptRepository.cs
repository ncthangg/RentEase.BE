using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using static RentEase.Common.Base.EnumType;

namespace RentEase.Data.Repository.Main
{

    public class AptRepository : GenericRepository<Apt>
    {
        public AptRepository()
        {
        }
        public AptRepository(RentEaseContext context) => _context = context;

        public async Task<PagedResult<Apt>> GetAll(int? approveStatusId, bool? status, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: (f =>   (!approveStatusId.HasValue || f.ApproveStatusId == approveStatusId) &&
                                (!status.HasValue || f.Status == status.Value)),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q
                         .Include(i => i.AptStatus)
                         .Include(i => i.AptCategory)
            );
        }
        public async Task<PagedResult<Apt>> GetByAccountId(
          string accountId, int? approveStatusId, int page, int pageSize, bool? status)
        {
            return await GetPagedAsync(
            filter: (f => f.PosterId == accountId &&
                        (approveStatusId == null || f.ApproveStatusId == approveStatusId) &&
                        (!status.HasValue || f.Status == status.Value)),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q
                         .Include(i => i.AptStatus)
                         .Include(i => i.AptCategory)
            );
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
