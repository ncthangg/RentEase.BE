
namespace RentEase.Common.DTOs.Dto;

public class RequestMaintenanceRequestDto
{
    public int AptId { get; set; }

    public string Description { get; set; }

    public int Priority { get; set; }

    public string? Note { get; set; }
}

public class ResponseMaintenanceRequestDto
{
    public int Id { get; set; }

    public int AptId { get; set; }

    public int LesseeId { get; set; }

    public string Description { get; set; }

    public int Priority { get; set; }

    public int AgentId { get; set; }

    public int ApproveStatusId { get; set; }

    public int ProgressStatusId { get; set; }

    public string Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}