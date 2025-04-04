﻿namespace RentEase.Common.DTOs.Authenticate
{
    public class RegisterReq
    {
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required int RoleId { get; set; }
    }

    public class RegisterRes
    {
        public required string FullName { get; set; }
        public required string Username { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
