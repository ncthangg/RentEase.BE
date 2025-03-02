
namespace RentEase.Common.DTOs.Dto;
public class RequestAptStatusDto
{
    public string StatusName { get; set; }

    public string? Description { get; set; }
}

public class ResponseAptStatusDto
{
    public int Id { get; set; }

    public string StatusName { get; set; }

    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}