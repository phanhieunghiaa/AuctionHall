using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories
{
    public class WalletRepository: IWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Wallets>> GetAllWallet()
        {
            return await _context.Wallets.ToListAsync();
        }
        public async Task<Wallets> GetWalletsById(string walletId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(sc => sc.ID == walletId);
        }

        public async Task<Wallets> GetWalletByUserIdAsync(string userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        }
        public async Task AddWallet(Wallets wallets)
        {
            _context.Wallets.Add(wallets);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateWallet(Wallets wallets)
        {
            _context.Wallets.Update(wallets);
            await _context.SaveChangesAsync();
        }
    }
}
