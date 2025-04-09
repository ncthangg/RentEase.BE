namespace RentEase.Data.Models;

public partial class Review
{
    public int Id { get; set; }

    public string AccountId { get; set; } = string.Empty;

    public string AptId { get; set; } = string.Empty;

    public double? Rating { get; set; }

    public string? Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Apt? Apt { get; set; }
}