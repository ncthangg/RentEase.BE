using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class Apt
{
    public string AptId { get; set; } = string.Empty;

    public string PosterId { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string OwnerPhone { get; set; } = string.Empty;

    public string OwnerEmail { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double? Area { get; set; }

    public string Address { get; set; } = string.Empty;

    public string AddressLink { get; set; } = string.Empty;

    public int ProvinceId { get; set; }

    public int DistrictId { get; set; }

    public int WardId { get; set; }

    public int AptCategoryId { get; set; }

    public int AptStatusId { get; set; }

    public int NumberOfRoom { get; set; }

    public int NumberOfSlot { get; set; }

    public string? Note { get; set; } = string.Empty;

    public double? Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }

    public virtual AptCategory? AptCategory { get; set; }

    public virtual ICollection<AptImage> AptImages { get; set; } = new List<AptImage>();

    public virtual ICollection<AptUtility> AptUtilities { get; set; } = new List<AptUtility>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<AccountLikedApt> AccountLikedApt { get; set; } = new List<AccountLikedApt>();
}