using System.Text.Json.Serialization;

namespace RentEase.Common.DTOs.Request
{
    public class PayOSWebhookRequest
    {
        [JsonPropertyName("order_code")]
        public string OrderCode { get; set; }  // Mã đơn hàng

        [JsonPropertyName("status")]
        public string Status { get; set; }  // Trạng thái thanh toán (CANCELLED, PAID, EXPIRED)

        [JsonPropertyName("message")]
        public string Message { get; set; }  // Lý do thay đổi trạng thái

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }  // Thời gian cập nhật trạng thái
    }

}
