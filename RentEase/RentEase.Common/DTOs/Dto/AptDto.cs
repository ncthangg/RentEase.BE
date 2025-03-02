namespace RentEase.Common.DTOs.Dto;

public class RequestAptDto
{
    public int OwnerId { get; set; }

    public string AptCode { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double? Area { get; set; }

    public string Address { get; set; }

    public string AddressLink { get; set; }

    public int ProvinceId { get; set; }

    public int DistrictId { get; set; }

    public int WardId { get; set; }

    public long RentPrice { get; set; }

    public long? PilePrice { get; set; }

    public int CategoryId { get; set; }

    public int StatusId { get; set; }

    public int NumberOfRoom { get; set; }

    public int AvailableRoom { get; set; }

    public int ApproveStatusId { get; set; }

    public string Note { get; set; }

    public double Rating { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; } = null;

    public DateTime? DeletedAt { get; set; } = null;

    public bool? Status { get; set; } = true;
}

public class ResponseAptDto
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public string AptCode { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public double? Area { get; set; }

    public string Address { get; set; }

    public string AddressLink { get; set; }

    public int ProvinceId { get; set; }

    public int DistrictId { get; set; }

    public int WardId { get; set; }

    public long RentPrice { get; set; }

    public long? PilePrice { get; set; }

    public int CategoryId { get; set; }

    public int StatusId { get; set; }

    public int NumberOfRoom { get; set; }

    public int AvailableRoom { get; set; }

    public int ApproveStatusId { get; set; }

    public string Note { get; set; }

    public double? Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public bool? Status { get; set; }
}