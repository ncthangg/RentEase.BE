namespace RentEase.Common.DTOs.Dto;
public class RequestWalletTransactionDto
{
    public string OrderId { get; set; }

    public decimal Amount { get; set; }

    public int TransactionTypeId { get; set; }

    public string Description { get; set; }
}
public class ResponseWalletTransactionDto
{
    public int Id { get; set; }

    public string OrderId { get; set; }

    public int WalletId { get; set; }

    public decimal Amount { get; set; }

    public int TransactionTypeId { get; set; }

    public int TransactionStatusId { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

}