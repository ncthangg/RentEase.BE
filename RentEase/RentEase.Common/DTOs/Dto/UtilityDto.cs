namespace RentEase.Common.DTOs.Dto
{
    public class RequestUtilityDto
    {
        public string UtilityName { get; set; }

        public string Description { get; set; }
    }
    public class ResponseUtilityDto
    {
        public int Id { get; set; }

        public string UtilityName { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool? Status { get; set; }
    }
}
