using Microsoft.EntityFrameworkCore;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository
{
    public class AccountTokenRepository : GenericRepository<AccountToken>
    {
        public AccountTokenRepository()
        {
        }
        public AccountTokenRepository(RentEaseContext context) => _context = context;

        public async Task<AccountToken> GetByAccountIdAndToken(int accountId, string refreshToken)
        {
            return await _context.Set<AccountToken>()
                .AsNoTracking()
                .FirstOrDefaultAsync(token => token.AccountId == accountId && token.RefreshToken == refreshToken);
        }

    }
}
