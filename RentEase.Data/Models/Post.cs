using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class Post
{
    public string PostId { get; set; } = string.Empty;

    public int PostCategoryId { get; set; }

    public string PosterId { get; set; } = string.Empty;

    public string AptId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public long RentPrice { get; set; }

    public long? PilePrice { get; set; }

    public int TotalSlot { get; set; }

    public int CurrentSlot { get; set; }

    public int GenderId { get; set; }

    public int OldId { get; set; }

    public string? Note { get; set; } = string.Empty;

    public DateOnly MoveInDate { get; set; }

    public DateOnly? MoveOutDate { get; set; }

    public int ApproveStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? StartPublic { get; set; }

    public DateTime? EndPublic { get; set; }

    public bool? Status { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Apt? Apt { get; set; }

    public virtual PostCategory? PostCategory { get; set; }

    public virtual ICollection<PostRequire> PostRequires { get; set; } = new List<PostRequire>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}