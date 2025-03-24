namespace RentEase.Common.DTOs.Dto
{
    public class OrderReq
    {
        public int OrderTypeId { get; set; }

        public string PostId { get; set; } = string.Empty;

        public decimal Amount { get; set; } = 0;

        public decimal IncurredCost { get; set; } = 0;
        
        public string Note { get; set; } = string.Empty;
    }

    public class OrderRes
    {
        public string OrderId { get; set; } = string.Empty;

        public int OrderTypeId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string PostId { get; set; } = string.Empty;

        public string SenderId { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Note { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public int PaymentStatusId { get; set; }
    }
}
