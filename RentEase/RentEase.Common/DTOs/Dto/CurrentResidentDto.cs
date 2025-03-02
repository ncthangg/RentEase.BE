
namespace RentEase.Common.DTOs.Dto;
public class RequestCurrentResidentDto
{
    public int AptId { get; set; }

    public int AccountId { get; set; }

    public DateTime MoveInDate { get; set; }

    public DateTime? MoveOutDate { get; set; }

    public int StatusId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; } = null;

    public DateTime? DeletedAt { get; set; } = null;

    public bool? Status { get; set; } = true;
}
public class ResponseCurrentResidentDto
{
    public int Id { get; set; }

    public int AptId { get; set; }

    public int AccountId { get; set; }

    public DateTime MoveInDate { get; set; }

    public DateTime? MoveOutDate { get; set; }

    public int StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}