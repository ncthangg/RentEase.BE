namespace RentEase.Common.DTOs.Dto
{
    public class PostReq
    {
        public int PostCategoryId { get; set; }

        public string AptId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public int TotalSlot { get; set; } = 0;

        public int CurrentSlot { get; set; } = 0;

        public long RentPrice { get; set; }

        public long? PilePrice { get; set; }

        public int GenderId { get; set; }

        public int OldId { get; set; }

        public string? Note { get; set; }

        public DateOnly MoveInDate { get; set; }

        public DateOnly? MoveOutDate { get; set; }

    }
    public class PostRes : Base
    {
        public string PostId { get; set; } = string.Empty;

        public int PostCategoryId { get; set; }

        public string PosterId { get; set; } = string.Empty;

        public string AptId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public long RentPrice { get; set; }

        public long? PilePrice { get; set; }

        public int TotalSlot { get; set; }

        public int CurrentSlot { get; set; }

        public int GenderId { get; set; }

        public int OldId { get; set; }

        public string Note { get; set; } = string.Empty;

        public DateOnly MoveInDate { get; set; }

        public DateOnly? MoveOutDate { get; set; }

        public DateTime? StartPublic { get; set; }

        public DateTime? EndPublic { get; set; }

    }
}
