using Microsoft.EntityFrameworkCore;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository
{
    public class AccountVerificationRepository : GenericRepository<AccountVerification>
    {
        public AccountVerificationRepository()
        {
        }
        public AccountVerificationRepository(RentEaseContext context) => _context = context;
        public async Task<AccountVerification> GetByAccountId(int accountId)
        {
            return await _context.Set<AccountVerification>()
                .AsNoTracking()
                .FirstOrDefaultAsync(token => token.AccountId == accountId);
        }
    }
}
