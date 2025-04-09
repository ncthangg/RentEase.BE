using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class AptUtility
{
    public int Id { get; set; }

    public string AptId { get; set; } = string.Empty;

    public int UtilityId { get; set; }

    public string? Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Apt? Apt { get; set; }

    public virtual Utility? Utility { get; set; }
}