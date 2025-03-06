namespace RentEase.Common.DTOs.Dto
{
    public class Base
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public bool? Status { get; set; }
    }
}
