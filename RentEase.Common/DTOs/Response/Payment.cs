using RentEase.Common.DTOs.Dto;

namespace RentEase.Common.DTOs.Response
{
    public class PaymentRes
    {
        public TransactionRes TransactionRes { get; set; } = new TransactionRes();
        public string Link { get; set; } = string.Empty;

    }
    public class VnPayRes
    {
        public bool Success { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderNote { get; set; }
        public string? OrderId { get; set; }
        public string? PaymentId { get; set; }
        public string? TransactionId { get; set; }
        public string? Token { get; set; }
        public string? VnPayResponseCode { get; set; }

    }
}
