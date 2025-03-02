using RentEase.Common.DTOs.Response;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Sub
{

    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository()
        {
        }
        public RoleRepository(RentEaseContext context) => _context = context;

        public async Task<PagedResult<Role>> GetAllAsync(bool? status, int page, int pageSize)
        {
            IQueryable<Role> query = _context.Set<Role>();

            Expression<Func<Role, bool>> filter = a =>
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }

        public async Task<PagedResult<Role>> GetBySearchAsync(string? roleName, bool? status, int page, int pageSize)
        {
            IQueryable<Role> query = _context.Set<Role>();

            Expression<Func<Role, bool>> filter = a =>
                (string.IsNullOrEmpty(roleName) || a.RoleName.Contains(roleName)) &&
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }
    }
}
