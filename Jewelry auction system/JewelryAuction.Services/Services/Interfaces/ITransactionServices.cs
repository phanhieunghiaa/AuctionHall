using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface ITransactionServices
    {
        Task<List<Transactions>> GetAllTransaction();
        Task<Transactions> GetTransactionsById(string transactionId);
        Task<List<Transactions>> GetAllTransactionsNyWalletId(string walletId);
        Task<List<Transactions>> GetAllTransactionsNyUserId(string userId);
        Task<string> AddTransaction(string walletId, TransactionDto transactions);


    }
}
