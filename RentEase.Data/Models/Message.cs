
namespace RentEase.Data.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public string ConversationId { get; set; } = string.Empty;
        public string SenderId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsSeen { get; set; }
        public virtual Conversation? Conversation { get; set; }
    }
}
