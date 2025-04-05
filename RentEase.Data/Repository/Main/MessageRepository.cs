using Microsoft.EntityFrameworkCore;
using RentEase.Common.DTOs;
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
    public class MessageRepository : GenericRepository<Message>
    {
        public MessageRepository()
        {
        }
        public MessageRepository(RentEaseContext context) => _context = context;

        public async Task<IEnumerable<Message>> GetByConversationIdAsync(string conversationId)
        {
            return await _context.Messages
                                 .Where(m => m.ConversationId.Contains(conversationId))
                                 .OrderBy(m => m.SentAt)
                                 .ToListAsync();
        }

    }
}
