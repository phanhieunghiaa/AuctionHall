using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface IFeedBackRepository
    {
        Task AddFeedback(FeedBacks feedBacks);
        Task UpdateFeedback(FeedBacks feedBacks);
        Task<bool> FeedbackExistsAsync(string userId, string auctionId);
        Task<FeedBacks> GetFeedbackByUserIdAndAuctionId(string userId, string auctionId);
        Task<FeedBacks> GetFeedbackById(string feedbackId);
        Task<IEnumerable<FeedBacks>> GetAllFeedbacksByAuctionId(string auctionId, FeedBackStatus status);
    }
}
