using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
using RentEase.Common.DTOs.Dto;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Main
{

    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository()
        {
        }
        public OrderRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<Order>> GetByAccountId(
            string accountId, int? statusId, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: o => o.SenderId == accountId &&
                             (statusId == null || o.PaymentStatusId == statusId),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize,
                includes: q => q.Include(i => i.TransactionType)
                );
        }

        public async Task<PagedResult<Order>> GetByStatusId(int? statusId, int page, int pageSize)
        {
            return await GetPagedAsync(
                 filter: f => statusId == null || f.PaymentStatusId == statusId,
                 orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                 page: page,
                 pageSize: pageSize,
                 includes: q => q.Include(i => i.TransactionType)
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
