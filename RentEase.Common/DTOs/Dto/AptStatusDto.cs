
namespace RentEase.Common.DTOs.Dto;
public class AptStatusReq
{
    public string StatusName { get; set; } = string.Empty;

    public string? Note { get; set; }
}

public class AptStatusRes : Base
{
    public int Id { get; set; }

    public string? StatusName { get; set; }

    public string? Note { get; set; }
}