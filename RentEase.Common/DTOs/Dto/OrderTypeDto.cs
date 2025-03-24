
namespace RentEase.Common.DTOs.Dto;
public class OrderTypeReq
{
    public string TypeName { get; set; } = string.Empty;

    public string? Note { get; set; }
}
public class OrderTypeRes : Base
{
    public int Id { get; set; }

    public string? TypeName { get; set; }

    public string? Note { get; set; }
}