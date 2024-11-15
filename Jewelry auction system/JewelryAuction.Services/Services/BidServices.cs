using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Background;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services
{
    public class BidServices : IBidServices
    {
        private readonly IBidRepository _bidRepository;
        private readonly IAuctionsRepository _auctionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<AuctionCheckService> _logger;

        public BidServices(IBidRepository bidRepository, IAuctionsRepository auctionsRepository,
            IWalletRepository walletRepository, IUserRepository userRepository,
            ILogger<AuctionCheckService> logger, IProductRepository productRepository)
        {
            _bidRepository = bidRepository;
            _auctionRepository = auctionsRepository;
            _walletRepository = walletRepository;
            _userRepository = userRepository;
            _logger = logger;
            _productRepository = productRepository;
        }

        #region Place Bid

        public async Task PlaceBid(string auctionId, string userId, float bidValue)
        {
            var auction = await _auctionRepository.GetSimpleAuction(auctionId);
            var user = await _userRepository.GetUserById(userId);
            if (auction == null)
            {
                throw new KeyNotFoundException("Cannot find this auction.");
            }
            else
            if (auction.Status != AuctionStatus.Active)
            {
                throw new KeyNotFoundException("Auction status must be active to bid.");
            }
            else
            if (user == null)
            {
                throw new KeyNotFoundException("Cannot find this user Id.");
            }
            if (auction.FinalPrice >= bidValue || auction.StartingPrice >= bidValue)
            {
                throw new KeyNotFoundException("Bid must be greater than the previous price.");
            }
            else
                        if (((bidValue - auction.FinalPrice) < 100) || ((bidValue - auction.StartingPrice) < 100))
            {
                throw new KeyNotFoundException("Bid must be greater than the previous price at least 100.");
            }
            else
            {
                var wallet = await _walletRepository.GetWalletByUserIdAsync(userId);
                if (wallet == null)
                {
                    throw new KeyNotFoundException("Cannot find this wallet.");
                }
                else
                if (wallet.Balance < bidValue)
                {
                    throw new KeyNotFoundException("Wallet balance must be greater than bid value.");
                }
                else
                {
                    var totalBids = await _bidRepository.GetTotalHighestBidValueByUserIdAsync(userId, BidStatus.Active);
                    if (totalBids > (wallet.Balance))
                    {
                        throw new KeyNotFoundException("Your wallet balance does not enough to bid.");
                    }
                    else
                    {

                        var bid = new Bids
                        {
                            AuctionId = auctionId,
                            UserId = userId,
                            Value = bidValue,
                            Status = BidStatus.Active
                        };

                        auction.StartingPrice = bidValue;

                        await _walletRepository.UpdateWallet(wallet);
                        await _auctionRepository.UpdateAuctions(auction);
                        await _bidRepository.AddBid(bid);
                    }
                }
            }
        }
        #endregion

        #region Cancel bid

        public async Task CancelBid(string bidId)
        {
            var bid = await _bidRepository.GetBidById(bidId);


            if (bid == null)
            {
                throw new KeyNotFoundException("Cannot find this bid.");
            }
            else
            if (bid.Status != BidStatus.Active)
            {
                throw new KeyNotFoundException("Bid status must be active to be cancel.");
            }
            else
            {
                bid.Status = BidStatus.Disable;
                await _bidRepository.UpdateBid(bid);

                var auction = await _auctionRepository.GetSimpleAuction(bid.AuctionId);
                var highestBid = await _bidRepository.GetHighestBidByAuctionId(bid.AuctionId, BidStatus.Active);
                if (highestBid == null)
                {
                    auction.StartingPrice = auction.BasePrice;
                    _logger.LogInformation($"auction starting price now is: {auction.StartingPrice}");
                }
                else
                {
                    auction.StartingPrice = highestBid.Value;
                    _logger.LogInformation($"auction starting price now is: {auction.StartingPrice}");
                }




                await _auctionRepository.UpdateAuctions(auction);

            }
        }
        #endregion

        #region View all bid by user ID

        public async Task<List<BidByUserIdDTO>> GetBidsByUserIdAsync(string userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Cannot find this user Id.");
            }
            else
            {
                var bids = await _bidRepository.GetBidsByUserIdAsync(userId);
                var bidDTOs = new List<BidByUserIdDTO>();


                foreach (var bid in bids)
                {
                    var auction = await _auctionRepository.GetSimpleAuction(bid.AuctionId);
                    var product = await _productRepository.GetProductbyIdAsync(auction.ProductId);
                    bidDTOs.Add(new BidByUserIdDTO
                    {
                        ID = bid.ID,
                        CreatedDate = bid.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        UpdatedDate = bid.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                        Value = bid.Value,
                        Status = Enum.GetName(typeof(BidStatus), bid.Status),
                        AuctionId = bid.AuctionId,
                        UserId = bid.UserId,
                        ProductName = product.Name,
                    });
                }
                return bidDTOs;
            }
        }
        #endregion

        #region View auction all bid

        public async Task<List<BidDTO>> GetBidsByAuctionIdAsync(string auctionId)
        {
            var auction = await _auctionRepository.GetSimpleAuction(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException("Cannot find this auction Id.");
            }
            else
            {
                var bids = await _bidRepository.GetBidsByAuctionIdAndStatusAsync(auctionId, BidStatus.Active);
                if (bids == null)
                {
                    throw new KeyNotFoundException("There is no bid found.");
                }
                else
                {
                    return bids.Select(b => new BidDTO
                    {
                        ID = b.ID,
                        CreatedDate = b.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                        UpdatedDate = b.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                        Value = b.Value,
                        Status = b.Status.ToString(),
                        AuctionId = Enum.GetName(typeof(BidStatus), b.Status),
                        UserId = b.UserId
                    }).ToList();
                }
            }
        }
        #endregion

        #region

        public async Task<HighestBidDTO> ViewHighestBidByAuctionId(string auctionId)
        {
            var auction = await _auctionRepository.GetSimpleAuction(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException("Cannot find this auction Id.");
            }
            else
            {
                var bid = await _bidRepository.GetHighestBidByAuctionId(auctionId, BidStatus.Active);
                if (bid == null)
                {
                    throw new KeyNotFoundException("There is no bid found.");
                }
                else
                {
                    var user = await _userRepository.GetUserById(bid.UserId);
                    return new HighestBidDTO
                    {
                        ID = bid.ID,
                        CreatedDate = bid.CreatedDate,
                        UpdatedDate = bid.UpdatedDate,
                        Value = bid.Value,
                        AuctionId = bid.AuctionId,
                        UserId = bid.UserId,
                        Status = bid.Status.ToString(),
                        UserFullName = user.FullName
                    };
                }

            }
        }
        #endregion

        #region

        public async Task<List<HighestBidDTO>> GetHighestBidsByUserIdAsync(string userId)
        {
            var highestBids = await _bidRepository.GetHighestBidsByUserIdAsync(userId);

            return highestBids.Select(b => new HighestBidDTO
            {
                ID = b.ID,
                CreatedDate = b.CreatedDate,
                UpdatedDate = b.UpdatedDate,
                Value = b.Value,
                AuctionId = b.AuctionId,
                UserId = b.UserId,
                Status = b.Status.ToString()
            }).ToList();
        }

        public async Task<Dictionary<string, float>> GetTotalBidValueByUserIdAsync(string userId)
        {
            return await _bidRepository.GetTotalBidValueByUserIdAsync(userId, BidStatus.Active);
        }

        public async Task<float> GetTotalHighestBidValueByUserIdAsync(string userId)
        {
            return await _bidRepository.GetTotalHighestBidValueByUserIdAsync(userId, BidStatus.Active);
        }
        #endregion
    }
}
