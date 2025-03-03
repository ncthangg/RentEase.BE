
namespace RentEase.Common.DTOs.Dto;
public class RequestWalletDto
{
    public int AccountId { get; set; }
}
public class ResponseWalletDto
{
    public int AccountId { get; set; }

    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}