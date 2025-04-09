using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class AccountVerification
{
    public int Id { get; set; }

    public string AccountId { get; set; } = string.Empty;

    public string VerificationCode { get; set; } = string.Empty;

    public bool? IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public virtual Account? Account { get; set; }
}