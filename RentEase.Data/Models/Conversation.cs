using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Data.Models
{
    public partial class Conversation
    {
        public string Id { get; set; }
        public string AccountId1 { get; set; }
        public string AccountId2 { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
