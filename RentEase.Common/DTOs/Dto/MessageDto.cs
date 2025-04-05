using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Common.DTOs.Dto
{
    public class MessageReq
    {
        public string ConversationId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class MessageRes
    {
        public int Id { get; set; }
        public string ConversationId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsSeen { get; set; }
    }
}
