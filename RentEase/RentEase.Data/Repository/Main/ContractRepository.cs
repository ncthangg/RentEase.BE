using RentEase.Common.DTOs.Response;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Main
{
    public class ContractRepository : GenericRepository<Contract>
    {
        public ContractRepository()
        {
        }
        public ContractRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<Contract>> GetAllAsync(bool? status, int page, int pageSize)
        {
            IQueryable<Contract> query = _context.Set<Contract>();

            Expression<Func<Contract, bool>> filter = a =>
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }
        //public async Task<PagedResult<Contract>> GetBySearchAsync(string? fullName, string? email, string? phoneNumber, int page, int pageSize)
        //{
        //    IQueryable<Contract> query = _context.Set<Contract>();

        //    Expression<Func<Contract, bool>> filter = a =>
        //        (string.IsNullOrEmpty(fullName) || a.FullName.Contains(fullName)) &&
        //        (string.IsNullOrEmpty(email) || a.Email.Contains(email)) &&
        //        (string.IsNullOrEmpty(phoneNumber) || a.PhoneNumber.Contains(phoneNumber));

        //    return await GetPagedAsync(filter, null, page, pageSize);
        //}
    }
}
