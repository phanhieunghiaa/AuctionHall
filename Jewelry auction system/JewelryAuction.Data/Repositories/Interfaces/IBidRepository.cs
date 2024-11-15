using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface IBidRepository
    {
        Task AddBid(Bids bid);
        Task UpdateBid(Bids bid);
        Task<List<Bids>> GetBidsByUserIdAsync(string userId);
        Task<Bids> GetBidByUserIdAndBidIdAsync(string userId, string bidId);
        Task<Bids> GetBidById(string bidId);
        Task<Bids> GetHighestBidByAuctionId(string auctionId, BidStatus status);
        Task<List<Bids>> GetBidsByAuctionIdAsync(string auctionId);
        Task<List<Bids>> GetBidsByAuctionIdAndStatusAsync(string auctionId, BidStatus status);
        Task<List<Bids>> GetHighestBidsByUserIdAsync(string userId);
        Task<Dictionary<string, float>> GetTotalBidValueByUserIdAsync(string userId, BidStatus status);
        Task<float> GetTotalHighestBidValueByUserIdAsync(string userId, BidStatus status);
        Task<Bids> GetHighestBidByAuctionAndUserIdAsync(string auctionId, string userId, BidStatus status);
        Task RemoveBidAsync(Bids bid);
    }
}
