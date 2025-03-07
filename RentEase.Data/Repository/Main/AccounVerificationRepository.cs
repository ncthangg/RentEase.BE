using Microsoft.EntityFrameworkCore;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{
    public class AccountVerificationRepository : GenericRepository<AccountVerification>
    {
        public AccountVerificationRepository()
        {
        }
        public AccountVerificationRepository(RentEaseContext context) => _context = context;

        public async Task<AccountVerification?> GetByAccountId(string accountId)
        {
            return await _context.Set<AccountVerification>()
                .Where(v => v.AccountId == accountId)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();
        }
        public async Task<AccountVerification?> GetByAccountIdAndVerificationCode(string accountId, string verificationCode)
        {
            return await _context.Set<AccountVerification>()
                .Where(v => v.AccountId == accountId && v.VerificationCode == verificationCode)
                .OrderByDescending(v => v.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
