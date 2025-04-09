using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class PostRequire
{
    public string Id { get; set; } = string.Empty;
    public string PostId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public int ApproveStatusId { get; set; }
    public string? RequestMessage { get; set; } = string.Empty;
    public string? ResponseMessage { get; set; } = string.Empty;
    public DateTime? ResponseAt { get; set; }
    public string? Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual Account? Account { get; set; }
    public virtual Post? Post { get; set; }
}