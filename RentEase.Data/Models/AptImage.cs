using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class AptImage
{
    public int Id { get; set; }

    public string AptId { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Apt? Apt { get; set; }
}