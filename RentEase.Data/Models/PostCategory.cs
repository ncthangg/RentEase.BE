namespace RentEase.Data.Models
{
    public partial class PostCategory
    {
        public int Id { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
