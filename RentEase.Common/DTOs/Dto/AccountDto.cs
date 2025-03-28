namespace RentEase.Common.DTOs.Dto;
public class PostAccountReq
{
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public int GenderId { get; set; }
    public int OldId { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
    public int RoleId { get; set; }
}
public class PutAccountReq
{
    public string FullName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public int GenderId { get; set; }
    public int OldId { get; set; }
    public string AvatarUrl { get; set; } = string.Empty;
}
public class AccountRes : Base
{
    public string AccountId { get; set; } = string.Empty;

    public string? FullName { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly? DateOfBirth { get; set; }

    public int? GenderId { get; set; }

    public int? OldId { get; set; }

    public string? AvatarUrl { get; set; }

    public int RoleId { get; set; }

    public bool? IsVerify { get; set; }
}