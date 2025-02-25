using Microsoft.EntityFrameworkCore;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using RentEase.Common.DTOs.Response;

namespace RentEase.Data.Repository
{
    public class AccountRepository : GenericRepository<Account>
    {
        public AccountRepository()
        {
        }
        public AccountRepository(RentEaseContext context) => _context = context;
        public async Task<Account> GetByEmailAsync(string email)
        {
            return await _context.Set<Account>()
            .Where(u => EF.Functions.Like(u.Email, $"%{email}%"))
            .SingleOrDefaultAsync();
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
