using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IFeedBackServices
    {
        Task AddFeedback(string userId, string auctionId, FeedBackContentDTO feedBackContentDTO);
        Task EditFeedBack(string userId, string auctionId, FeedBackContentDTO feedBackContentDTO);
        Task DisableFeedBack(string userId, string auctionId);
        Task<FeedBackDTO> GetFeedbackById(string feedbackId);
        Task<IEnumerable<FeedBackDTO>> GetAllFeedbacksByAuctionId(string auctionId);
    }
}
