namespace RentEase.Common.DTOs.Dto
{
    public class UtilityReq
    {
        public string UtilityName { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
    public class UtilityRes : Base
    {
        public int Id { get; set; }

        public string? UtilityName { get; set; }

        public string? Note { get; set; }
    }
}
