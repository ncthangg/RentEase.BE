
namespace RentEase.Common.DTOs.Dto;
public class RequestTransactionTypeDto
{
    public string TypeName { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; } = null;

    public DateTime? DeletedAt { get; set; } = null;

    public bool? Status { get; set; } = true;
}
public class ResponseTransactionTypeDto
{
    public int Id { get; set; }

    public string TypeName { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}