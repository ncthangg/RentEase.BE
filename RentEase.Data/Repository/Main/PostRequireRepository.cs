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
        //public async Task<PagedResult<PostRequire>> GetAllOwn(
        //          string accountId, int page = 1, int pageSize = 10)
        //{
        //    return await GetPagedAsync(
        //        filter: o => o.SenderId == accountId,
        //        orderBy: q => q.PostRequireByDescending(o => o.CreatedAt),
        //        page: page,
        //        pageSize: pageSize);
        //}

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
