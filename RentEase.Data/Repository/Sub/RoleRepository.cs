using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;

namespace RentEase.Data.Repository.Sub
{

    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository()
        {
        }
        public RoleRepository(RentEaseContext context) => _context = context;


    }
}
