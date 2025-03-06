namespace RentEase.Common.DTOs.Dto;
public class TransactionReq
{
    public string OrderId { get; set; } = string.Empty;
    public string? Note { get; set; }
}
public class TransactionRes
{
    public int Id { get; set; }

    public int TransactionTypeId { get; set; }

    public string OrderId { get; set; } = string.Empty;

    public int PaymentAttempt { get; set; }

    public string PaymentCode { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? PaidAt { get; set; }

    public int StatusId { get; set; }

}