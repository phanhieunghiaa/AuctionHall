using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface IAuctionsRepository
    {
        Task<List<Auctions>> GetAllAuctions(string searchterm);
        Task AddAuctions(Auctions auctions);
        Task<Auctions> GetAuctionsbyID(string id);
        Task UpdateAuctions(Auctions auctions);
        Task DeleteAuctions(string id);
        Task<Auctions> GetAuctionWithDetailsAsync(string auctionId);
        Task<List<Auctions>> GetAuctionsByStatusAndSearchTermAsync(AuctionStatus status, string? searchTerm);
        Task<Auctions> GetSimpleAuction(string auctionId);
    }
}
