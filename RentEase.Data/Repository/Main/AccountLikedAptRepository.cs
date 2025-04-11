using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{
    public class AccountLikedAptRepository : GenericRepository<AccountLikedApt>
    {
        public AccountLikedAptRepository()
        {
        }
        public AccountLikedAptRepository(RentEaseContext context) => _context = context;

        public async Task<PagedResult<AccountLikedApt>> GetByAccountId(
                                            string accountId, bool status, int page = 1, int pageSize = 10)
        {
            return await GetPagedAsync(
                filter: (f => f.AccountId == accountId && f.Apt.Status == status) ,
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q
                         .Include(i => i.Apt)
                );
        }

        public async Task<bool> RemoveByAccountIdAndAptIdAsync(string accountId, string aptId)
        {
            var entity = await _context.AccountLikedApts
            .FirstOrDefaultAsync(x => x.AccountId == accountId && x.AptId == aptId);

            if (entity == null)
                return false;

            _context.AccountLikedApts.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAllByAccountIdAsync(string accountId)
        {
            var entities = _context.AccountLikedApts.Where(x => x.AccountId == accountId);
            _context.AccountLikedApts.RemoveRange(entities);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
