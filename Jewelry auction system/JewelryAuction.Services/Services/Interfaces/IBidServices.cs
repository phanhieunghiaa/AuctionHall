using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IBidServices
    {
        Task PlaceBid(string auctionId, string userId, float bidValue);
        Task CancelBid(string bidId);
        Task<List<BidByUserIdDTO>> GetBidsByUserIdAsync(string userId);
        Task<List<BidDTO>> GetBidsByAuctionIdAsync(string auctionId);
        Task<HighestBidDTO> ViewHighestBidByAuctionId(string auctionId);
        Task<List<HighestBidDTO>> GetHighestBidsByUserIdAsync(string userId);
        Task<Dictionary<string, float>> GetTotalBidValueByUserIdAsync(string userId);
        Task<float> GetTotalHighestBidValueByUserIdAsync(string userId);
    }
}
