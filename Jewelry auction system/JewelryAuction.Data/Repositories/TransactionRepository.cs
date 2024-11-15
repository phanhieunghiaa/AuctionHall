using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace JewelryAuction.Data.Repositories
{
    public class TransactionRepository: ITransactionRepository
    {
        private readonly ApplicationDbContext _context;
        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transactions>> GetAllTransaction()
        {
            return await _context.Transactions.ToListAsync();
        }
        public async Task<Transactions> GetTransactionById(string transactionId)
        {
            return await _context.Transactions.FirstOrDefaultAsync(sc => sc.ID == transactionId);
        }
        
        public async Task<List<Transactions>> GetAllTransactionsByWalletId(string walletId)
        {
            return await _context.Transactions.Where(w => w.WalletId == walletId).OrderByDescending(o => o.CreatedDate).ToListAsync();
        }

        public async Task<List<Transactions>> GetAllTransactionsByUserId(string userId)
        {
            return await _context.Transactions.Where(sc => sc.Wallets.UserId == userId).OrderByDescending(o => o.CreatedDate).ToListAsync();
        }
        public async Task AddTransaction(Transactions transactions)
        {
            _context.Transactions.Add(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTransaction(Transactions transactions)
        {
            _context.Transactions.Update(transactions);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTransaction(string transactionId)
        {
            Transactions transactions = await GetTransactionById(transactionId);
            if (transactions != null)
            {
                _context.Remove(transactions);
                await _context.SaveChangesAsync();
            }
        }
    }
}
