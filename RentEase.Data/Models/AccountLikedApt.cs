namespace RentEase.Data.Models
{
    public partial class AccountLikedApt
    {
        public int Id { get; set; }

        public string AccountId { get; set; } = string.Empty;

        public string AptId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public virtual Account? Account { get; set; }

        public virtual Apt? Apt { get; set; }
    }
}
