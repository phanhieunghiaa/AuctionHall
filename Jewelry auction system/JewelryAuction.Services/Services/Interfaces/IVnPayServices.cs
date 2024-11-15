using JewelryAuction.Data.Models.ResponseDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IVnPayServices
    {
        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model, string userid);
        VnPaymentResponseModel PaymentExecute(Dictionary<string, string> url);
        public string GetUserId(string orderInfo);
        public string GetTransactionId(string orderInfo);
    }
}
