using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories
{
    public  class AuctionsRepository: IAuctionsRepository
    {
        private readonly ApplicationDbContext _context;
        public AuctionsRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<List<Auctions>> GetAllAuctions(string searchterm)
        {
            var query = _context.Auctions.AsQueryable();
            if(searchterm != null)
            {
                query = query.Include(sc => sc.Products).Where(sc => sc.Products.Name.Contains(searchterm));
                return await query.ToListAsync();
            }
            return await query.ToListAsync();
        }

        public async Task AddAuctions(Auctions auctions)
        {
            _context.Auctions.Add(auctions);
            await _context.SaveChangesAsync();
        }

        public async Task<Auctions> GetAuctionsbyID(string id)
        {
            return await _context.Auctions.FirstOrDefaultAsync(sc => sc.ID.Equals(id));
        }

        public async Task UpdateAuctions(Auctions auctions)
        {
            _context.Auctions.Update(auctions);
            await _context.SaveChangesAsync();  
        }
        public async Task DeleteAuctions(string id)
        {
            Auctions auctions = await GetAuctionsbyID(id);
            if (auctions != null)
            {
                _context.Auctions.Remove(auctions);
                await _context.SaveChangesAsync();
            }
        }


        #region Get auction detail include img

        public async Task<Auctions> GetAuctionWithDetailsAsync(string Id)
        {
            var au = await _context.Auctions.Include(a => a.Products).ThenInclude(p => p.Images).FirstOrDefaultAsync(a => a.ID == Id);
            return au;
        }
        #endregion

        #region Get all auction with img

        public async Task<List<Auctions>> GetAuctionsByStatusAndSearchTermAsync(AuctionStatus status, string? searchTerm)
        {
            IQueryable<Auctions> query = _context.Auctions.Include(a => a.Products).ThenInclude(p => p.Images).Where(a => a.Status == status);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Products.Name.Contains(searchTerm));
            }

            return await query.ToListAsync();
        }
        #endregion

        #region Sub code

        public async Task<Auctions> GetSimpleAuction(string auctionId)
        {
            var au = await _context.Auctions.Include(a => a.Products).FirstOrDefaultAsync(a => a.ID == auctionId);
            return au;
        }
        #endregion
    }
}
