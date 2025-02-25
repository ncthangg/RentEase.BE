
namespace RentEase.Common.DTOs.Dto;
public class RequestRoleDto
{
    public string RoleName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; } = null;
}
public class ResponseRoleDto
{
    public int Id { get; set; }

    public string RoleName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}