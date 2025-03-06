namespace RentEase.Common.DTOs.Dto;
public class AccountReq
{
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public int RoleId { get; set; }
}
public class AccountRes : Base
{
    public string AccountId { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }
}