using RentEase.Common.DTOs.Response;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Data.Repository.Main
{

    public class OrderRepository : GenericRepository<Order>
    {
        public OrderRepository()
        {
        }
        public OrderRepository(RentEaseContext context) => _context = context;
        //public async Task<PagedResult<Order>> GetAllAsync(bool? status, int page, int pageSize)
        //{
        //    IQueryable<Order> query = _context.Set<Order>();

        //    Expression<Func<Order, bool>> filter = a =>
        //        (!status.HasValue || a.Status == status.Value);

        //    return await GetPagedAsync(filter, null, page, pageSize);
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
