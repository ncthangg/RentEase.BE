using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Sub
{
    public class UtilityRepository : GenericRepository<Utility>
    {
        public UtilityRepository()
        {
        }
        public UtilityRepository(RentEaseContext context) => _context = context;


    }
}
