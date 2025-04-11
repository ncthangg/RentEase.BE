using Microsoft.EntityFrameworkCore;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Sub
{

    public class OrderTypeRepository : GenericRepository<OrderType>
    {
        public OrderTypeRepository()
        {
        }
        public OrderTypeRepository(RentEaseContext context) => _context = context;
        public async Task<List<OrderType>> GetListByPostCategoryId(int postCategoryId)
        {
            return await _context.Set<OrderType>()
                .Where(p => p.PostCategoryId == postCategoryId)
                .ToListAsync();
        }

        public async Task<OrderType?> GetByPostCategoryId(int postCategoryId)
        {
            return await _context.Set<OrderType>()
                .Where(p => p.PostCategoryId == postCategoryId)
                .FirstOrDefaultAsync();
        }

    }
}
