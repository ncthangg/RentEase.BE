namespace RentEase.Common.DTOs.Dto
{
    public class PostRequireReq
    {
        public string PostId { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
    public class PostRequireRes
    {
        public int Id { get; set; }

        public string AccountId { get; set; } = string.Empty;

        public string PostId { get; set; } = string.Empty;

        public string? Note { get; set; }

        public int ApproveStatusId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
