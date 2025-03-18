namespace RentEase.Data.Models
{
    public partial class PostCategory
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }

        public string Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
