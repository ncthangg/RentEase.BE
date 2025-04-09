namespace RentEase.Common.DTOs.Dto
{
    public class PostRequireReq
    {
        public string PostId { get; set; } = string.Empty;
        public string? RequestMessage { get; set; } = string.Empty;
    }
    public class PutRequireReq
    {
        public string PostId { get; set; } = string.Empty;
        public int ApproveStatusId { get; set; } 
        public string? ResponseMessage { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
    }
    public class PostRequireRes
    {
        public string Id { get; set; } = string.Empty;
        public string PostId { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public int ApproveStatusId { get; set; }
        public string? RequestMessage { get; set; } = string.Empty;
        public string? ResponseMessage { get; set; } = string.Empty;
        public DateTime? ResponseAt { get; set; }
        public string? Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
