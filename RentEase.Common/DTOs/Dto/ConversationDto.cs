using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEase.Common.DTOs.Dto
{
    public class ConversationReq
    {
        public string AccountIdReceive { get; set; } = string.Empty;
    }

    public class ConversationRes
    {
        public string Id { get; set; } = string.Empty;
        public string AccountId1 { get; set; } = string.Empty;
        public string AccountId2 { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
