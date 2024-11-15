using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IWalletServices
    {
        Task<List<Wallets>> GetAllWallets();
        Task<Wallets> GetWalletById(string walletId);
        Task<Wallets> GetWalletByUserId(string UserId);
        Task<string> AddWallet(string userId);
        Task<string> UpdateWallets(string walletsId, WalletRequestDTO walletRequestDTO);
        Task UpdateWalletBalanceAsync(string transactionId);
        Task UpdateWalleBalance(PaymentRequestDTO paymentRequest);
    }
}
