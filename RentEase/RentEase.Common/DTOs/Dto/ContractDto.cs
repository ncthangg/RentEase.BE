
namespace RentEase.Common.DTOs.Dto;
public class RequestContractDto
{
    public int AptId { get; set; }

    public int LessorId { get; set; }

    public int LesseeId { get; set; }

    public int? AgentId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int RentPrice { get; set; }

    public int? PilePrice { get; set; }

    public string FileUrl { get; set; }

    public int ContractStatusId { get; set; }

    public int ApproveStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; } = null;

    public DateTime? DeletedAt { get; set; } = null;

    public bool? Status { get; set; } = true;
}
public class ResponseContractDto
{
    public int Id { get; set; }

    public int AptId { get; set; }

    public int LessorId { get; set; }

    public int LesseeId { get; set; }

    public int? AgentId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int RentPrice { get; set; }

    public int? PilePrice { get; set; }

    public string FileUrl { get; set; }

    public int ContractStatusId { get; set; }

    public int ApproveStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}