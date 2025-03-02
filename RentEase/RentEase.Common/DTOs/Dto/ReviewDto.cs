
namespace RentEase.Common.DTOs.Dto;
public class RequestReviewDto
{
    public int AptId { get; set; }

    public double? Rating { get; set; }

    public string? Comment { get; set; }
}
public class ResponseReviewDto
{
    public int Id { get; set; }

    public int ReviewerId { get; set; }

    public int AptId { get; set; }

    public double? Rating { get; set; }

    public string Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}