using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class AptCategory
{
    public int Id { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Apt> Apts { get; set; } = new List<Apt>();
}