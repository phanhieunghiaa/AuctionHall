using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services
{
    public class WalletServices: IWalletServices
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IUserRepository _userRepository;

        public WalletServices(IWalletRepository walletRepository, ApplicationDbContext dbContext, IUserRepository userRepository)
        {
            _walletRepository = walletRepository;
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task<List<Wallets>> GetAllWallets()
        {
            return await _walletRepository.GetAllWallet();
        }

        public async Task<Wallets> GetWalletById(string walletId)
        {
            if(walletId == null)
            {
                throw new ArgumentException("Wallet not found");
            }
            return await _walletRepository.GetWalletsById(walletId);
        }
        public async Task<Wallets> GetWalletByUserId(string UserId)
        {
            if(UserId == null)
            {
                throw new ArgumentException("User not found");
            }
            return await _walletRepository.GetWalletByUserIdAsync(UserId);
        }

        public async Task<string> AddWallet(string userId)
        {
            //if (walletDTO == null)
            //{
            //    return "Wallet not Found";
            //}
            var existingWallet = await GetWalletByUserId(userId);
            var user = await _userRepository.GetUserById(userId);
            if (existingWallet != null)
            {
                return "A wallet with the UserID already exists.";
            }
            Wallets newWallets = new Wallets()
            {
                ID = Guid.NewGuid().ToString(),
                Balance = 0,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                UpdatedBy = user.FullName,
                UserId = userId,
            };
            await _walletRepository.AddWallet(newWallets);
            return "Success";
        }

        public async Task<string> UpdateWallets(string walletsId, WalletRequestDTO walletRequestDTO)
        {       
            var wallet = await _walletRepository.GetWalletsById(walletsId);
            if (wallet == null)
            {
                return "Wallet not found";
            }
            wallet.Balance = walletRequestDTO.Balance;  
            wallet.CreatedDate = DateTime.Now;
            wallet.UpdatedDate = DateTime.Now;
            wallet.UpdatedBy = walletRequestDTO.UpdatedBy;
            await _walletRepository.UpdateWallet(wallet);
            return "Success";
        }

        public async Task UpdateWalletBalanceAsync(string transactionId)
        {
            var transaction = await _dbContext.Transactions
                .Include(t => t.Wallets)
                .FirstOrDefaultAsync(t => t.ID == transactionId);

            if (transaction != null && transaction.Status == "Completed")
            {
                transaction.Wallets.Balance += transaction.Amount;
                _dbContext.Update(transaction.Wallets);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateWalleBalance(PaymentRequestDTO paymentRequest)
        {
            var transaction = await _dbContext.Transactions
            .Include(t => t.Wallets)
            .FirstOrDefaultAsync(t => t.ID == paymentRequest.TransactionId);

            if (transaction != null)
            {
                transaction.Wallets.Balance += (float)paymentRequest.Amount;
                _dbContext.Update(transaction.Wallets);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
