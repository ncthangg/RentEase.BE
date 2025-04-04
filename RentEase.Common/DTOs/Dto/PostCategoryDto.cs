﻿
namespace RentEase.Common.DTOs.Dto;
public class PostCategoryReq
{
    public string CategoryName { get; set; } = string.Empty;

    public string Note { get; set; } = string.Empty;
}
public class PostCategoryRes : Base
{
    public int Id { get; set; }

    public string? CategoryName { get; set; }

    public string? Note { get; set; }
}