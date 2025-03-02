namespace RentEase.Common.DTOs.Dto
{
    public class RequestAptUtilityDto
    {
        public int AptId { get; set; }

        public int UtilityId { get; set; }

        public string Description { get; set; }
    }
    public class ResponseAptUtilityDto
    {
        public int Id { get; set; }

        public int AptId { get; set; }

        public int UtilityId { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
        public bool? Status { get; set; }
    }
}
