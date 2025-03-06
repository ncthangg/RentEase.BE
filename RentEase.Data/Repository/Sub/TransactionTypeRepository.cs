using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

namespace RentEase.Data.Repository.Sub
{

    public class TransactionTypeRepository : GenericRepository<TransactionType>
    {
        public TransactionTypeRepository()
        {
        }
        public TransactionTypeRepository(RentEaseContext context) => _context = context;

        public async Task<PagedResult<TransactionType>> GetAllAsync(bool? status, int page, int pageSize)
        {
            IQueryable<TransactionType> query = _context.Set<TransactionType>();

            Expression<Func<TransactionType, bool>> filter = null;

            return await GetPagedAsync(filter, null, page, pageSize);
        }

    }
}
