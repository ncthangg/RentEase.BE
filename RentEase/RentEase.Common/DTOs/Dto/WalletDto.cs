
namespace RentEase.Common.DTOs.Dto;
public class RequestWalletDto
{
    public int AccountId { get; set; }

    public decimal Balance { get; set; } = 0;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; } = null;

    public DateTime? DeletedAt { get; set; } = null;

    public bool? Status { get; set; } = true;
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