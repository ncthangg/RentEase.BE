using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

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
