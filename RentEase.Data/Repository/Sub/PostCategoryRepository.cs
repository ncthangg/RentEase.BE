using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Sub
{
    public class PostCategoryRepository : GenericRepository<PostCategory>
    {
        public PostCategoryRepository()
        {
        }
        public PostCategoryRepository(RentEaseContext context) => _context = context;

    }
}
