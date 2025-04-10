namespace RentEase.Common.DTOs.Dto
{
    public class AccountLikedAptReq
    {
        public string AptId { get; set; } = string.Empty;
    }

    public class AccountLikedAptRes
    {
        public int Id { get; set; }

        public string AccountId { get; set; } = string.Empty;

        public string AptId { get; set; } = string.Empty;

        public DateTime CreateAt { get; set; }
    }
}
