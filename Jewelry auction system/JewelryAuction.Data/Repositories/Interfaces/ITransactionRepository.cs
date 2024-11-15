using Jewelry_auction_system.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transactions>> GetAllTransaction();
        Task<Transactions> GetTransactionById(string transactionId);
        Task<List<Transactions>> GetAllTransactionsByWalletId(string walletId);
        Task<List<Transactions>> GetAllTransactionsByUserId(string userId);
        Task AddTransaction(Transactions transactions);
        Task UpdateTransaction(Transactions transactions);
        Task DeleteTransaction(string transactionId);
    }
}
