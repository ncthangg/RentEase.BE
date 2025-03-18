namespace RentEase.Common.DTOs.Dto;

public class AptReq
{
    public string OwnerId { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string OwnerPhone { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double Area { get; set; } = 0;

    public string Address { get; set; } = string.Empty;

    public string AddressLink { get; set; } = string.Empty;

    public int ProvinceId { get; set; }

    public int DistrictId { get; set; }

    public int WardId { get; set; }

    public long RentPrice { get; set; } = 0;

    public long PilePrice { get; set; } = 0;

    public int AptCategoryId { get; set; }

    public int AptStatusId { get; set; } = 1;

    public int NumberOfRoom { get; set; } = 0;

    public int NumberOfSlot { get; set; } = 0;

    public string? Note { get; set; }
}

public class AptRes : Base
{
    public string AptId { get; set; } = string.Empty;

    public string OwnerId { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string OwnerPhone { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double? Area { get; set; }

    public string Address { get; set; } = string.Empty;

    public string AddressLink { get; set; } = string.Empty;

    public int ProvinceId { get; set; }

    public int DistrictId { get; set; }

    public int WardId { get; set; }

    public long RentPrice { get; set; }

    public long? PilePrice { get; set; }

    public int AptCategoryId { get; set; }

    public int AptStatusId { get; set; }

    public int NumberOfRoom { get; set; }

    public int NumberOfSlot { get; set; }

    public int ApproveStatusId { get; set; }

    public string? Note { get; set; }

    public double? Rating { get; set; }
}