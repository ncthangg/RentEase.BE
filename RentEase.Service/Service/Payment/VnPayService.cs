﻿using Microsoft.Extensions.Configuration;
using RentEase.Data;

namespace RentEase.Service.Service.Payment
{
    public interface IVnpayService
    {
        //Task<string> CreatePaymentURL(string orderCode, decimal amount);
        //VnPayRes PaymentExecute(IQueryCollection collections);
    }
    public class VnpayService : IVnpayService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        public VnpayService(IConfiguration config)
        {
            _unitOfWork ??= new UnitOfWork();
            _config = config;
        }
        //public async Task<string> CreatePaymentURL(string paymentCode, decimal amount)
        //{

        //    //Get Config Info
        //    string VNP_URL = _config["VnpaySettings:VNP_URL"];
        //    string VNP_TMNCODE = _config["VnpaySettings:VNP_TMNCODE"];
        //    string VNP_HASHSECRET = _config["VnpaySettings:VNP_HASHSECRET"];

        //    if (string.IsNullOrEmpty(VNP_TMNCODE) || string.IsNullOrEmpty(VNP_HASHSECRET))
        //    {
        //        return "Vui lòng cấu hình các tham số: vnp_TmnCode,vnp_HashSecret trong file web.config";
        //    }

        //    string VNP_AMOUNT = ((int)(amount * 100)).ToString();
        //    string VNP_COMMAND = _config["VnpaySettings:VNP_COMMAND"];
        //    string VNP_VERSION = _config["VnpaySettings:VNP_VERSION"];
        //    string VNP_CREATEDATE = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    string VNP_CURRCODE = _config["VnpaySettings:VNP_CURRCODE"];
        //    string VNP_IPADDR = "127.0.0.1";
        //    string VNP_LOCATE = _config["VnpaySettings:VNP_LOCATE"];
        //    string VNP_ORDERINFO = "Thanh toan don hang " + paymentCode;
        //    string VNP_ORDERTYPE = "order";
        //    string VNP_RETURNURL = _config["VnpaySettings:VNP_RETURNURL" ?? ""];

        //    // Lấy số lần thanh toán và tạo TxnRef duy nhất
        //    string VNP_TXNREF = paymentCode;


        //    var vnpay = new VnPayLibrary();
        //    vnpay.AddRequestData("vnp_Version", VNP_VERSION);
        //    vnpay.AddRequestData("vnp_Command", VNP_COMMAND);
        //    vnpay.AddRequestData("vnp_TmnCode", VNP_TMNCODE);
        //    vnpay.AddRequestData("vnp_Amount", VNP_AMOUNT);
        //    vnpay.AddRequestData("vnp_CreateDate", VNP_CREATEDATE);
        //    vnpay.AddRequestData("vnp_CurrCode", VNP_CURRCODE);
        //    vnpay.AddRequestData("vnp_IpAddr", VNP_IPADDR);
        //    vnpay.AddRequestData("vnp_Locale", VNP_LOCATE);

        //    vnpay.AddRequestData("vnp_OrderInfo", VNP_ORDERINFO);
        //    vnpay.AddRequestData("vnp_OrderType", VNP_ORDERTYPE);
        //    vnpay.AddRequestData("vnp_ReturnUrl", VNP_RETURNURL);

        //    vnpay.AddRequestData("vnp_TxnRef", VNP_TXNREF);

        //    var paymentUrl = vnpay.CreateRequestUrl(VNP_URL, VNP_HASHSECRET);

        //    return paymentUrl;

        //}

        //public VnPayRes PaymentExecute(IQueryCollection collections)
        //{
        //    var vnpay = new VnPayLibrary();
        //    foreach (var (key, value) in collections)
        //    {
        //        if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
        //        {
        //            vnpay.AddResponseData(key, value.ToString());
        //        }
        //    }

        //    string vnp_txnRef = vnpay.GetResponseData("vnp_TxnRef");
        //    var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
        //    var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
        //    var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        //    var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

        //    bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnpaySettings:VNP_HASHSECRET"]);
        //    if (!checkSignature)
        //    {
        //        return new VnPayRes
        //        {
        //            Success = false
        //        };
        //    }

        //    return new VnPayRes
        //    {
        //        Success = true,
        //        PaymentMethod = "VnPay",
        //        OrderNote = vnp_OrderInfo,
        //        OrderId = vnp_txnRef,
        //        PaymentId = vnp_txnRef,
        //        TransactionId = vnp_TransactionId.ToString(),
        //        Token = vnp_SecureHash,
        //        VnPayResponseCode = vnp_ResponseCode
        //    };
        //}

    }
}
