using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Sub
{
    public class AptCategoryRepository : GenericRepository<AptCategory>
    {
        public AptCategoryRepository()
        {
        }
        public AptCategoryRepository(RentEaseContext context) => _context = context;


    }
}
