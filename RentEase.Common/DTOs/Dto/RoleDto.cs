
namespace RentEase.Common.DTOs.Dto;
public class RoleReq
{
    public string RoleName { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;
}
public class RoleRes : Base
{
    public int Id { get; set; }

    public string? RoleName { get; set; }

    public string? Note { get; set; }
}