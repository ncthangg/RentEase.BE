namespace RentEase.Common.DTOs.Dto;

public class RequestAptCategoryDto
{
    public string CategoryName { get; set; }

    public string Description { get; set; }
}
public class ResponseAptCategoryDto
{
    public int Id { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}