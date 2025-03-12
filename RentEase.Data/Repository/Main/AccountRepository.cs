using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Main
{
    public class AccountRepository : GenericRepository<Account>
    {
        public AccountRepository()
        {
        }
        public AccountRepository(RentEaseContext context) => _context = context;
        public async Task<Account?> GetByEmailAsync(string email)
        {
            return await _context.Set<Account>()
            .Include(x => x.Role)
            .Where(u => EF.Functions.Like(u.Email, $"%{email}%"))
            .SingleOrDefaultAsync();
        }
        public async Task<Account?> GetByPhoneAsync(string phone)
        {
            return await _context.Set<Account>()
            .Include(x => x.Role)
            .Where(u => EF.Functions.Like(u.PhoneNumber, $"%{phone}%"))
            .SingleOrDefaultAsync();
        }
        public async Task<Account?> GetByEmailOrPhoneAsync(string username)
        {
            return await _context.Set<Account>().FirstOrDefaultAsync(a => a.Email == username || a.PhoneNumber == username);
        }
        public async Task<PagedResult<Account>> GetBySearchAsync(string? fullName, string? email, string? phoneNumber, bool? isActive, bool? status, int page, int pageSize)
        {
            IQueryable<Account> query = _context.Set<Account>();

            Expression<Func<Account, bool>> filter = a =>
                (string.IsNullOrEmpty(fullName) || a.FullName.Contains(fullName)) &&
                (string.IsNullOrEmpty(email) || a.Email.Contains(email)) &&
                (string.IsNullOrEmpty(phoneNumber) || a.PhoneNumber.Contains(phoneNumber)) &&
                (!isActive.HasValue || a.IsActive == isActive.Value) &&
                (!status.HasValue || a.Status == status.Value); ;

            return await GetPagedAsync(filter, null, page, pageSize);
        }
    }


}
