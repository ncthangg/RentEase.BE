using RentEase.Common.DTOs;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System.Linq.Expressions;

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
