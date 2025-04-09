namespace RentEase.Data.Models;

public partial class AccountToken
{
    public int Id { get; set; }

    public string AccountId { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public virtual Account? Account { get; set; }
}