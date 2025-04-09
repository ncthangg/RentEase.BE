using System.Text.Json.Serialization;

namespace RentEase.Common.DTOs.Request
{
    public class PayOSWebhookRequest
    {
        [JsonPropertyName("order_code")]
        public string OrderCode { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }  // Thời gian cập nhật trạng thái
    }

}
