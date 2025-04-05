using RentEase.Data.DBContext;
using RentEase.Data.Models;
using RentEase.Data.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Data.Repository.Main
{
    public class ConversationRepository : GenericRepository<Conversation>
    {
        public ConversationRepository()
        {
        }
        public ConversationRepository(RentEaseContext context) => _context = context;

    }
}
