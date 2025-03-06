
namespace RentEase.Common.DTOs.Dto;
public class ReviewReq
{
    public string AptId { get; set; } = string.Empty;

    public double? Rating { get; set; }

    public string Comment { get; set; } = string.Empty;
}
public class ReviewRes : Base
{
    public int Id { get; set; }

    public string ReviewerId { get; set; } = string.Empty;

    public string AptId { get; set; } = string.Empty;

    public double? Rating { get; set; }

    public string Comment { get; set; } = string.Empty;
}