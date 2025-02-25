namespace RentEase.Common.DTOs.Dto;
public class RequestWalletTransactionDto
{
    public int AccountId { get; set; }

    public decimal Amount { get; set; }

    public int TransactionTypeId { get; set; }

    public int TransactionStatusId { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
public class ResponseWalletTransactionDto
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public decimal Amount { get; set; }

    public int TransactionTypeId { get; set; }

    public int TransactionStatusId { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }
}