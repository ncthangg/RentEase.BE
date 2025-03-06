using Microsoft.EntityFrameworkCore;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{

    public class TransactionRepository : GenericRepository<Transaction>
    {
        public TransactionRepository()
        {
        }
        public TransactionRepository(RentEaseContext context) => _context = context;
        public async Task<Transaction> GetByOrderCode(string orderId)
        {
            return await _context.Set<Transaction>()
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<Transaction> GetByPaymentCode(string paymentCode)
        {
            return await _context.Set<Transaction>()
                .Where(p => p.PaymentCode == paymentCode)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();
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
