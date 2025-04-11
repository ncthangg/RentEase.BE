using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class Account
{
    public string AccountId { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public int? GenderId { get; set; }

    public int? OldId { get; set; }

    public string? AvatarUrl { get; set; }

    public int RoleId { get; set; }

    public bool? IsVerify { get; set; }

    public int PublicPostTimes {  get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }

    public virtual ICollection<AccountToken> AccountTokens { get; set; } = new List<AccountToken>();

    public virtual ICollection<AccountVerification> AccountVerifications { get; set; } = new List<AccountVerification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<PostRequire> PostRequires { get; set; } = new List<PostRequire>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<AccountLikedApt> AccountLikedApt { get; set; } = new List<AccountLikedApt>();

    public virtual Role? Role { get; set; }
}