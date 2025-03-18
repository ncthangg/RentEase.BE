namespace RentEase.Common.DTOs.Dto
{
    public class PostReq
    {
        public int PostCategoryId { get; set; }

        public string AptId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public int TotalSlot { get; set; } = 0;

        public int CurrentSlot { get; set; } = 0;

        public int GenderId { get; set; }

        public int OldId { get; set; }

        public string? Note { get; set; }

        public DateOnly MoveInDate { get; set; }

        public DateOnly? MoveOutDate { get; set; }
        public int ApproveStatusId { get; set; } = 1;
    }
    public class PostRes : Base
    {
        public string PostId { get; set; } = string.Empty;

        public int PostCategoryId { get; set; }

        public string AccountId { get; set; } = string.Empty;

        public string AptId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public int TotalSlot { get; set; }

        public int CurrentSlot { get; set; }

        public int GenderId { get; set; }

        public int OldId { get; set; }

        public string? Note { get; set; }

        public DateOnly MoveInDate { get; set; }

        public DateOnly? MoveOutDate { get; set; }

        public int ApproveStatusId { get; set; }
    }
}
