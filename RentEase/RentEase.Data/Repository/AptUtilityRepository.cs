using RentEase.Common.DTOs.Response;
using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Data.Repository
{
    public class AptUtilityRepository : GenericRepository<AptUtility>
    {
        public AptUtilityRepository()
        {
        }
        public AptUtilityRepository(RentEaseContext context) => _context = context;
    }
}
