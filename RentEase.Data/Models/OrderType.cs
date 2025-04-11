using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class OrderType
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;

    public int Times { get; set; }

    public int Days { get; set; }

    public decimal Amount { get; set; }

    public int PostCategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}