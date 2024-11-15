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
    public class FeedBackRepository : IFeedBackRepository
    {

        private readonly ApplicationDbContext _context;

        public FeedBackRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task AddFeedback(FeedBacks feedBacks)
        {
            _context.Add(feedBacks);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFeedback(FeedBacks feedBacks)
        {
            _context.Update(feedBacks);
            await _context.SaveChangesAsync();
        }

        public async Task<FeedBacks> GetFeedbackByUserIdAndAuctionId(string userId, string auctionId)
        {
            var fb = await _context.FeedBacks.FirstOrDefaultAsync(f => f.UserId == userId && f.AuctionId == auctionId);
            return fb;
        }

        public async Task<FeedBacks> GetFeedbackById(string feedbackId)
        {
            var fb = await _context.FeedBacks.FindAsync(feedbackId);
            return fb;
        }

        public async Task<IEnumerable<FeedBacks>> GetAllFeedbacksByAuctionId(string auctionId, FeedBackStatus status)
        {
            return await _context.FeedBacks.Where(f => f.AuctionId == auctionId && f.Status == status).ToListAsync();

        }
        #endregion

        #region Check if feedback exist

        public async Task<bool> FeedbackExistsAsync(string userId, string auctionId)
        {
            return await _context.FeedBacks.AnyAsync(f => f.UserId == userId && f.AuctionId == auctionId);
        }
        #endregion
    }
}
