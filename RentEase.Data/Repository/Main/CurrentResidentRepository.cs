using RentEase.Common.DTOs.Response;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Main
{
    public class CurrentResidentRepository : GenericRepository<CurrentResident>
    {
        public CurrentResidentRepository()
        {
        }
        public CurrentResidentRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<CurrentResident>> GetAllAsync(bool? status, int page, int pageSize)
        {
            IQueryable<CurrentResident> query = _context.Set<CurrentResident>();

            Expression<Func<CurrentResident, bool>> filter = a =>
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }
        public async Task<PagedResult<CurrentResident>> GetAllForAptAsync(
                          int aptId, bool? status, int page = 1, int pageSize = 10)
        {
            return await GetPagedAsync(
                filter: (o => o.AptId == aptId && (status == null || o.Status == status)),
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
