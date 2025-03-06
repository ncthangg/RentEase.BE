
namespace RentEase.Common.DTOs.Dto;
public class TransactionTypeReq
{
    public string TypeName { get; set; } = string.Empty;

    public string? Note { get; set; }
}
public class TransactionTypeRes : Base
{
    public int Id { get; set; }

    public string? TypeName { get; set; }

    public string? Note { get; set; }
}