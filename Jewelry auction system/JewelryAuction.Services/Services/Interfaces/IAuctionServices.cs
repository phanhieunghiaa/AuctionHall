using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IAuctionServices
    {
        Task<List<AuctionRequestDTO>> GetAllProducts(string searchterm);
        Task<Auctions> GetProductsByID(string id);
        Task<string> AddAuction(string productsID, AuctionsDTO auctionsDTO);
        Task<string> UpdateAuction(string auctionId, AuctionUpdateDTO auctionsDTO);
        Task<bool> UpdateStatusAuction(string auctionId, AuctionStatus newStatus);
        Task<AuctionDetailsDTO> GetAuctionDetailsAsync(string auctionId);
        Task<List<AuctionDetailsDTO>> GetAuctionsByStatusAndSearchTermAsync(AuctionStatus status, string searchTerm);

    }
}
