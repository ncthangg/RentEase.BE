﻿using RentEase.Common.DTOs.Dto;

namespace RentEase.Common.DTOs.Response
{
    public class PaymentRes
    {
        public OrderRes? OrderRes { get; set; } = new OrderRes();
        public PayosRes? PayosRes { get; set; } = new PayosRes();

    }
    public class PaymentCallback
    {
        public required string Code { get; set; } // Mã lỗi
        public required string Id { get; set; } // Payment Link Id
        public bool Cancel { get; set; } // Trạng thái hủy
        public required string Status { get; set; } // Trạng thái thanh toán
        public required string OrderCode { get; set; } // Mã đơn hàng
    }


    public class PayosRes
    {
        public string? Code { get; set; }
        public string? Desc { get; set; }
        public PayosData? Data { get; set; }
        public string? Signature { get; set; }
    }

    public class PayosData
    {
        public string? Bin { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountName { get; set; }
        public long Amount { get; set; }
        public string? Description { get; set; }
        public long OrderCode { get; set; }
        public string? Currency { get; set; }
        public string? PaymentLinkId { get; set; }
        public string? Status { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? QrCode { get; set; }
    }



}
