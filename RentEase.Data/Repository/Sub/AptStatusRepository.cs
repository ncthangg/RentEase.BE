using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Sub
{
    public class AptStatusRepository : GenericRepository<AptStatus>
    {
        public AptStatusRepository()
        {
        }
        public AptStatusRepository(RentEaseContext context) => _context = context;

    }
}
