namespace RentEase.Common.DTOs.Dto;

public class AptCategoryReq
{
    public string CategoryName { get; set; } = string.Empty;

    public string? Note { get; set; }
}
public class AptCategoryRes : Base
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public string? Note { get; set; }
}