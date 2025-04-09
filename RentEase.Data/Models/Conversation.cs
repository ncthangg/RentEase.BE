
namespace RentEase.Data.Models
{
    public partial class Conversation
    {
        public string Id { get; set; } = string.Empty;
        public string AccountId1 { get; set; } = string.Empty;
        public string AccountId2 { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
