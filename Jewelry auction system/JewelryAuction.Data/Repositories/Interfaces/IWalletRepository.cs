using Jewelry_auction_system.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        Task<List<Wallets>> GetAllWallet();
        Task<Wallets> GetWalletsById(string walletId);
        Task<Wallets> GetWalletByUserIdAsync(string userId);
        Task AddWallet(Wallets wallets);
        Task UpdateWallet(Wallets wallets);
    }
}
