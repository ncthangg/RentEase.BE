using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
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
        public async Task<PagedResult<Order>> GetAllAsync(int? paymentStatusId, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: o => (paymentStatusId == null || o.PaymentStatusId == paymentStatusId),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize
                );
        }

        public async Task<PagedResult<Order>> GetByAccountIdAsync(
            string accountId, int? paymentStatusId, int page, int pageSize)
        {
            return await GetPagedAsync(
                filter: o => (accountId == null || o.SenderId == accountId) &&
                             (paymentStatusId == null || o.PaymentStatusId == paymentStatusId),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt),
                page: page,
                pageSize: pageSize
                );
        }

        public async Task<Order?> GetByOrderCodeAsync(string orderCode)
        {
            return await _context.Set<Order>()
                .Where(p => p.OrderCode == orderCode)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<Order?> GetByOrderTypeIdAndPostIdAsync(string orderTypeId, string postId)
        {
            return await _context.Set<Order>()
                .Where(p => p.OrderTypeId == orderTypeId && p.PostId == postId)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
        }


    }
}
