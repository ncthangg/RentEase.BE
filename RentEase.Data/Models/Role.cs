namespace RentEase.Data.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}