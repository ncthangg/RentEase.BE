
namespace RentEase.Common.DTOs.Dto;
public class RequestContractDto
{
    public int AptId { get; set; }

    public int LessorId { get; set; }

    public int LesseeId { get; set; }

    public int? AgentId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public long RentPrice { get; set; }

    public long PilePrice { get; set; }

    public string FileUrl { get; set; }
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

    public long RentPrice { get; set; }

    public long PilePrice { get; set; }

    public string FileUrl { get; set; }

    public int ContractStatusId { get; set; }

    public int ApproveStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}