namespace RentEase.Common.DTOs.Dto;

public class RequestAptImageDto
{
    public int AptId { get; set; }

    public string? ImageUrl1 { get; set; }

    public string? ImageUrl2 { get; set; }

    public string? ImageUrl3 { get; set; }

    public string? ImageUrl4 { get; set; }

    public string? ImageUrl5 { get; set; }

    public string? ImageUrl6 { get; set; }
}
public class ResponseAptImageDto
{
    public int Id { get; set; }

    public int AptId { get; set; }

    public string ImageUrl1 { get; set; }

    public string ImageUrl2 { get; set; }

    public string ImageUrl3 { get; set; }

    public string ImageUrl4 { get; set; }

    public string ImageUrl5 { get; set; }

    public string ImageUrl6 { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}