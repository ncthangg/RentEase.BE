
namespace RentEase.Data.Models;

public partial class Order
{
    public string OrderId { get; set; } = string.Empty;

    public string OrderTypeId { get; set; } = string.Empty;

    public string OrderCode { get; set; } = string.Empty;

    public string PostId { get; set; } = string.Empty;

    public string SenderId { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string? Note { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public int PaymentStatusId { get; set; }

    public virtual Post? Post { get; set; }

    public virtual Account? Sender { get; set; }

    public virtual OrderType? OrderType { get; set; }

}