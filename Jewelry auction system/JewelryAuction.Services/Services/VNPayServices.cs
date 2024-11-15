﻿using JewelryAuction.Data.Helper;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Background;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JewelryAuction.Services.Services
{

    public class VNPayServices : IVnPayServices
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<VNPayServices> _logger;
        public VNPayServices(IConfiguration config, IUserRepository userRepository, ILogger<VNPayServices> logger)
        {
            _configuration = config;
            _userRepository = userRepository;
            _logger = logger;

        }
        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, string userid)
        {
            var user = _userRepository.GetUserByIdNoAsync(userid);
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            string payBackUrl = _configuration["VnPay:PaymentBackUrl"] + $"{user.ID}";

            vnpay.AddRequestData("vnp_Version", _configuration["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _configuration["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + $"TransactionId:{model.TransactionId},OrderId:{model.OrderId},Type:{model.Description},UserID:{user.ID},Anount:{model.Amount}");
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VnPay:PaymentBackUrl"]);
            _logger.LogInformation($"Next mid night: {payBackUrl}.");
            vnpay.AddRequestData("vnp_TxnRef", model.OrderId.ToString());
            //vnpay.AddRequestData("vnp_BankCode","ACB");
            
            TimeZoneInfo timeZone = TimeZoneInfo.Utc;
            DateTime utcTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeZone);
            // Thêm 7 giờ và 15 phút vào thời gian hiện tại
            DateTime expireTime = utcTime.AddHours(7).AddMinutes(15);
            // Định dạng thời gian theo yêu cầu
            string vnp_ExpireDate = expireTime.ToString("yyyyMMddHHmmss");
            vnpay.AddRequestData("vnp_ExpireDate", vnp_ExpireDate);

            var paymentUrl = vnpay.CreateRequestUrl(_configuration["VnPay:BaseUrl"], _configuration["VnPay:HashSecret"]);
            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(Dictionary<string, string> url)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in url)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }
            // Lấy dữ liệu từ vnpay, sử dụng GetResponseData mà không cần gọi ToString() vì dữ liệu đã là string
            var vnp_OrderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = url.TryGetValue("vnp_SecureHash", out var secureHash) ? secureHash : string.Empty;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            // Kiểm tra chữ ký
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]);

            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    //Message = "Invalid signature."
                };
            }

            // Chuyển đổi giá trị số lượng tiền từ chuỗi sang số (nếu cần)
            bool parseSuccess = decimal.TryParse(vnp_Amount, out decimal amount);

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_OrderId,
                TransactionId = vnp_TransactionId,
                Token = vnp_SecureHash,
                /*Amount = parseSuccess ? amount : 0,*/ // Sử dụng giá trị được phân tích nếu thành công, ngược lại sử dụng 0
                VnPayResponseCode = vnp_ResponseCode
            };
        }


        public string GetUserId(string orderInfo)
        {
            string details = orderInfo.Substring(orderInfo.IndexOf(':') + 1);

            // Tách các cặp khóa-giá trị
            var keyValuePairs = details.Split(',');

            // Dictionary để lưu các cặp khóa-giá trị
            var dict = new Dictionary<string, string>();

            foreach (var pair in keyValuePairs)
            {
                var keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    dict[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }

            // Lấy UserID và Amount từ dictionary
            if (dict.TryGetValue("UserID", out string userId))
            {
                return  userId;
            }
            else
            {
               return null;
            }
        }
        public string GetTransactionId(string orderInfo)
        {
            string details = orderInfo.Substring(orderInfo.IndexOf(':') + 1);

            // Tách các cặp khóa-giá trị
            var keyValuePairs = details.Split(',');

            // Dictionary để lưu các cặp khóa-giá trị
            var dict = new Dictionary<string, string>();

            foreach (var pair in keyValuePairs)
            {
                var keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    dict[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }

            // Lấy UserID và Amount từ dictionary
            if (dict.TryGetValue("TransactionId", out string transactionId))
            {
                return transactionId;
            }
            else
            {
                return null;
            }
        }

    }

}
