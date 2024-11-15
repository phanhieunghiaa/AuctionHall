using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Helper;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Services.Services;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionServices _services;
        private readonly IVnPayServices _vpnPayServices;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IWalletServices _walletServices;

        public TransactionsController(ITransactionServices services, IVnPayServices vpnPayServices, ApplicationDbContext dbContext, IWalletServices walletServices, IUserService userService)
        {
            _services = services;
            _vpnPayServices = vpnPayServices;
            _dbContext = dbContext;
            _walletServices = walletServices;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTransaction()
        {
            var result = await _services.GetAllTransaction();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet("Transactions/walletId")]
        public async Task<IActionResult> GetAllTransactionNyWalletId(string WalletId)
        {
            var transactions = await _services.GetAllTransactionsNyWalletId(WalletId);
            if (transactions == null )
            {
                return NotFound("No transactions found for the specified wallet ID.");
            }
            return Ok(transactions);
        }

        [HttpGet("Transactions/userId")]
        public async Task<IActionResult> GetAllTransactionNyUserId(string UserId)
        {
            var transactions = await _services.GetAllTransactionsNyUserId(UserId);
            if (transactions == null)
            {
                return NotFound("No transactions found for the specified userId ID.");
            }
            return Ok(transactions);
        }

        [HttpPost("payment/vnpay")]
        public async Task<IActionResult> AddTransaction(string payment, TransactionRequestDTO transactionDto, string userId)
        {
            try
            {
                var transaction = new TransactionDto();
                if (payment == "VnPay")
                {
                    var vnPayModel = new VnPaymentRequestModel()
                    {
                        Amount = transactionDto.Amount,
                        CreatedDate = DateTime.Now,
                        Description = "thanh toán VnPay",
                        TransactionId = Guid.NewGuid().ToString(),
                        OrderId = new Random().Next(1000, 10000),
                        FullName = "A"

                    };
                    if(vnPayModel.Amount < 0)
                    {
                        return BadRequest("The amount entered cannot be less than 0. Please try again");
                    }
                    var paymentUrl = _vpnPayServices.CreatePaymentUrl(HttpContext, vnPayModel, userId);
                    return Ok(new { url = paymentUrl });
                    //return Redirect(_vpnPayServices.CreatePaymentUrl(HttpContext, vnPayModel, userId));
                    //return new JsonResult(_vpnPayServices.CreatePaymentUrl(HttpContext, vnPayModel, userId));
                }
                return BadRequest("Payment not successful");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("PaymentBack")]
        public async Task<IActionResult> PaymenCalltBack()
        {
            var queryParameters = HttpContext.Request.Query;
            // Kiểm tra và lấy giá trị 'vnp_OrderInfo' từ Query
            string orderInfo = queryParameters["vnp_OrderInfo"];
            string userId = _vpnPayServices.GetUserId(orderInfo);
            string transactionId = _vpnPayServices.GetTransactionId(orderInfo);
            double amount = double.Parse(queryParameters["vnp_Amount"]);
            if (string.IsNullOrEmpty(orderInfo))
            {
                return BadRequest("Thông tin đơn hàng không tồn tại.");
            }
            // Phân tích chuỗi 'orderInfo' để lấy các thông tin cần thiết
            var orderInfoDict = new Dictionary<string, string>();
            string[] pairs = orderInfo.Split(',');
            foreach (var pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    orderInfoDict[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }
            // Lấy ví tiền của người dùng dựa trên 'UserID'
            var wallet = await _walletServices.GetWalletByUserId(userId);
            if (wallet == null)
            {
                return BadRequest("Wallet not found for the given user.");
            }
            // Tạo và lưu trữ thông tin giao dịch     
            var transactiondto = new TransactionDto()
            {
                TransactionId = transactionId,
                Status = "Completed",
                Amount =(float)amount / 100,  // Chia cho 100 nếu giá trị 'amount' là theo đơn vị nhỏ nhất của tiền tệ
                Payment = "VnPay",
                CreatedDate = DateTime.Now,
            };
            var result = await _services.AddTransaction(wallet.ID, transactiondto);
            if (result == "Success")
            {
                await _walletServices.UpdateWalletBalanceAsync(transactionId);
                return Redirect("https://meet.google.com/fcd-wvxs-cvn?authuser=1" + userId);
            }
            return BadRequest("Invalid transaction data.");
        }
    }
}
