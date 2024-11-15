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
    public class BidRepository : IBidRepository
    {
        private readonly ApplicationDbContext _context;

        public BidRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region CRUD bid

        public async Task AddBid(Bids bid)
        {
            _context.Add(bid);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBid(Bids bid)
        {
            _context.Update(bid);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Bids>> GetBidsByUserIdAsync(string userId)
        {
            return await _context.Bids.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<Bids> GetBidByUserIdAndBidIdAsync(string userId, string bidId)
        {
            var bid = await _context.Bids.FirstOrDefaultAsync(b => b.UserId == userId && b.ID == bidId);
            return bid;
        }

        public async Task<Bids> GetBidById(string bidId)
        {
            var bid = await _context.Bids.FindAsync(bidId);
            return bid;
        }

        public async Task<Bids> GetHighestBidByAuctionId(string auctionId, BidStatus status)
        {
            var bid = await _context.Bids.Where(b => b.AuctionId == auctionId && b.Status == status).OrderByDescending(b => b.Value).FirstOrDefaultAsync();
            return bid;
        }

        public async Task<List<Bids>> GetBidsByAuctionIdAsync(string auctionId)
        {
            return await _context.Bids.Where(b => b.AuctionId == auctionId).ToListAsync();
        }

        public async Task<List<Bids>> GetBidsByAuctionIdAndStatusAsync(string auctionId, BidStatus status)
        {
            return await _context.Bids.Where(b => b.AuctionId == auctionId && b.Status == status).ToListAsync();
        }

        public async Task<List<Bids>> GetHighestBidsByUserIdAsync(string userId)
        {
            var highestBids = await _context.Bids.Where(b => b.UserId == userId).GroupBy(b => b.AuctionId)
                .Select(g => g.OrderByDescending(b => b.Value).FirstOrDefault()).ToListAsync();

            return highestBids;
        }

        public async Task<Dictionary<string, float>> GetTotalBidValueByUserIdAsync(string userId, BidStatus status)
        {
            var highestBids = await _context.Bids
                .Where(b => b.UserId == userId && b.Status == status)
                .GroupBy(b => b.AuctionId)
                .Select(g => new
                {
                    AuctionId = g.Key,
                    TotalValue = g.Sum(b => b.Value)
                })
                .ToDictionaryAsync(x => x.AuctionId, x => x.TotalValue);

            return highestBids;
        }

        public async Task<float> GetTotalHighestBidValueByUserIdAsync(string userId, BidStatus status)
        {
            var highestBids = await _context.Bids
                .Where(b => b.UserId == userId && b.Status == status)
                .GroupBy(b => b.AuctionId)
                .Select(g => g.OrderByDescending(b => b.Value).FirstOrDefault())
                .ToListAsync();

            return highestBids.Sum(b => b.Value);
        }

        public async Task<Bids> GetHighestBidByAuctionAndUserIdAsync(string auctionId, string userId, BidStatus status)
        {
            return await _context.Bids
                .Where(b => b.AuctionId == auctionId && b.UserId == userId && b.Status == status)
                .OrderByDescending(b => b.Value)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveBidAsync(Bids bid)
        {
            _context.Bids.Remove(bid);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region


        #endregion
    }
}
