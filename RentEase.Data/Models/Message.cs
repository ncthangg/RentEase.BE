using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Data.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsSeen { get; set; }
        public virtual Conversation Conversation { get; set; }
    }
}
