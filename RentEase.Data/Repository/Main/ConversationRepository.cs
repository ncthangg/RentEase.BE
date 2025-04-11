using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Conversation>> GetByAccountIdAsync(string accountId)
        {
            return await _context.Conversations
                                 .Where(m => m.AccountId1 == accountId || m.AccountId2 == accountId)
                                 .OrderBy(m => m.CreatedAt)
                                 .ToListAsync();
        }
        public async Task<Conversation?> GetByReceiverIdAsync(string senderId, string receiverId)
        {
            return await _context.Conversations
                                 .Where(m => (m.AccountId1 == senderId && m.AccountId2 == receiverId) || (m.AccountId1 == receiverId && m.AccountId2 == senderId))
                                 .FirstOrDefaultAsync();
        }
        
    }
}
