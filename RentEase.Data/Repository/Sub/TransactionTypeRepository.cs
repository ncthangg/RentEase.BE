using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Sub
{

    public class TransactionTypeRepository : GenericRepository<TransactionType>
    {
        public TransactionTypeRepository()
        {
        }
        public TransactionTypeRepository(RentEaseContext context) => _context = context;


    }
}
