namespace RentEase.Common.DTOs.Dto;

public class AptImageReq
{
    public string AptId { get; set; } = string.Empty;

    public string ImageUrl1 { get; set; } = string.Empty;

    public string ImageUrl2 { get; set; } = string.Empty;

    public string ImageUrl3 { get; set; } = string.Empty;

    public string ImageUrl4 { get; set; } = string.Empty;

    public string ImageUrl5 { get; set; } = string.Empty;

    public string ImageUrl6 { get; set; } = string.Empty;
}
public class AptImageRes : Base
{
    public string AptId { get; set; } = string.Empty;

    public string? ImageUrl1 { get; set; }

    public string? ImageUrl2 { get; set; }

    public string? ImageUrl3 { get; set; }

    public string? ImageUrl4 { get; set; }

    public string? ImageUrl5 { get; set; }

    public string? ImageUrl6 { get; set; }

}