namespace RentEase.Common.DTOs.Dto;

public class RequestAptCategoryDto
{
    public string CategoryName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; } = null;

    public DateTime? DeletedAt { get; set; } = null;

    public bool? Status { get; set; } = true;
}
public class ResponseAptCategoryDto
{
    public int Id { get; set; }

    public string CategoryName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}