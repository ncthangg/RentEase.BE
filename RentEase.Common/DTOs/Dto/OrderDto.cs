namespace RentEase.Common.DTOs.Dto
{
    public class OrderReq
    {
        public int TransactionTypeId { get; set; }

        public decimal Amount { get; set; } = 0;

        public decimal IncurredCost { get; set; } = 0;
    }
    public class OrderRes
    {
        public string OrderId { get; set; } = string.Empty;

        public int TransactionTypeId { get; set; }

        public string SenderId { get; set; } = string.Empty;

        public decimal Amount { get; set; } = 0;

        public decimal IncurredCost { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }

        public int StatusId { get; set; }
    }
}
