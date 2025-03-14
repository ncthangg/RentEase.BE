using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace RentEase.Common.DTOs.Dto;

public class AptImageReq
{
    public string AptId { get; set; } = string.Empty;

    public List<IFormFile> Files { get; set; } = new List<IFormFile>();
}

public class AptImageRes : Base
{
    public int Id { get; set; }

    public string AptId { get; set; } = string.Empty;

    public List<Image> ImageUrl { get; set; } = new List<Image>();

}

public class Image
{
    public string ImageUrl { get; set; } = string.Empty;
}