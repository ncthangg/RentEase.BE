﻿namespace RentEase.Common.DTOs.Dto
{
    public class AptUtilityReq
    {
        public string AptId { get; set; } = string.Empty;

        public List<UtilityItem> Utilities { get; set; } = new List<UtilityItem>();
    }
    public class UtilityItem
    {
        public int UtilityId { get; set; }
        public string? Note { get; set; }
    }

    public class AptUtilityRes : Base
    {
        public int Id { get; set; }

        public string AptId { get; set; } = string.Empty;

        public int UtilityId { get; set; }

        public string? Note { get; set; }
    }
}
