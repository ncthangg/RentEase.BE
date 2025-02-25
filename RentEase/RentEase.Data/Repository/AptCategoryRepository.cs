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

namespace RentEase.Data.Repository
{
    public class AptCategoryRepository : GenericRepository<AptCategory>
    {
        public AptCategoryRepository()
        {
        }
        public AptCategoryRepository(RentEaseContext context) => _context = context;
        public async Task<PagedResult<AptCategory>> GetBySearchAsync(string? categoryName, bool? status, int page, int pageSize)
        {
            IQueryable<AptCategory> query = _context.Set<AptCategory>();

            Expression<Func<AptCategory, bool>> filter = a =>
                (string.IsNullOrEmpty(categoryName) || a.CategoryName.Contains(categoryName)) &&
                (!status.HasValue || a.Status == status.Value);

            return await GetPagedAsync(filter, null, page, pageSize);
        }
    }
}
