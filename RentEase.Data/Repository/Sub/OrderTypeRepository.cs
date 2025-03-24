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


    }
}
