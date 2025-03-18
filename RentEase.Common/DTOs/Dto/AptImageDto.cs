using Microsoft.AspNetCore.Http;

namespace RentEase.Common.DTOs.Dto;

public class PostListAptImageReq
{
    public string AptId { get; set; } = string.Empty;

    public List<IFormFile> Files { get; set; } = new List<IFormFile>();
}
public class PostSingleAptImageReq
{
    public string AptId { get; set; } = string.Empty;

    public required IFormFile File { get; set; }
}

public class AptImageRes
{
    public string AptId { get; set; } = string.Empty;
    public List<Image> Images { get; set; } = new List<Image>();
}

public class Image
{
    public int Id { get; set; }
    public required string ImageUrl { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}
