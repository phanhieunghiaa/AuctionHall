using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Data.Repositories;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services
{
    public class AuctionsServices: IAuctionServices
    {
        private readonly IAuctionsRepository _auctionsRepository;
        private readonly IProductRepository _productRepository;
        public AuctionsServices(IAuctionsRepository auctionsRepository, IProductRepository productRepository)
        {
            _auctionsRepository = auctionsRepository;
            _productRepository = productRepository;
        }

        public async Task<List<AuctionRequestDTO>> GetAllProducts(string searchterm)
        {
            List<AuctionRequestDTO> auctionsDTOs = new List<AuctionRequestDTO>();
            var AuctionList = await _auctionsRepository.GetAllAuctions(searchterm);
            foreach (var auction in AuctionList)
            {
                var product = await _productRepository.GetProductsById(auction.ProductId);
                var AuctionNeed = new AuctionRequestDTO()
                {
                    ID = auction.ID,
                    StartDate = auction.StartDate,
                    EndDate = auction.EndDate,
                    Status = auction.Status,
                    StartingPrice = auction.StartingPrice,
                    FinalPrice = (float)auction.FinalPrice,
                    Description = auction.Description,
                    Approve = auction.Approve,
                    Title = auction.Title,
                    Detail = auction.Detail,
                    ProductId = auction.ProductId,
                    ProductName = product.Name,
                };
                auctionsDTOs.Add(AuctionNeed);
            }
            return auctionsDTOs;
        }

        public async Task<Auctions> GetProductsByID(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("Auctions not found");
            }
            else
            {
                return await _auctionsRepository.GetAuctionsbyID(id);
            }
        }

        public async Task<string> AddAuction(string productsID, AuctionsDTO auctionsDTO)
        {
            string[] dateFormats = { "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy" };
            DateTime startdate;
            DateTime currentDate = DateTime.Today;
            if (!DateTime.TryParseExact(auctionsDTO.StartDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out startdate))
            {
                throw new ArgumentException("Invalid start startdate format", nameof(auctionsDTO.StartDate));
            }
            if (startdate <= currentDate)
            {
                throw new ArgumentException("Start date must be at least tomorrow");
            }
            else
            {
                var product = await _productRepository.GetProductsById(productsID);
                if (product == null)
                {
                    throw new Exception("Product not found");
                }
                if (product.Status != Data.Enums.ProductStatus.Requesting)
                {
                    throw new Exception("Product status must be Requesting to create auction");
                }
                else
                {
                    if (auctionsDTO == null)
                    {
                        return "Auctions not found";
                    }
                    Auctions newauctions = new Auctions()
                    {
                        StartDate = startdate,
                        EndDate = startdate.AddDays(7),
                        Status = 0,
                        BasePrice = auctionsDTO.StartingPrice,
                        StartingPrice = auctionsDTO.StartingPrice,
                        FinalPrice = 0,
                        Description = auctionsDTO.Description,
                        Approve = auctionsDTO.Approve,
                        Title = auctionsDTO.Title,
                        Detail = auctionsDTO.Detail,
                        ProductId = productsID,
                    };
                    await _auctionsRepository.AddAuctions(newauctions);
                    await _productRepository.UpdateStatusProduct(productsID, Data.Enums.ProductStatus.Accepted);
                }
                return "Success";
            }
        }

        public async Task<string> UpdateAuction(string auctionId, AuctionUpdateDTO auctionsDTO)
        {
            string[] dateFormats = { "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy" };
            DateTime startdate, endDate;
            if (!DateTime.TryParseExact(auctionsDTO.StartDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out startdate))
            {
                throw new ArgumentException("Invalid start startdate format", nameof(auctionsDTO.StartDate));
            }
            if (!DateTime.TryParseExact(auctionsDTO.EndDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                throw new ArgumentException("Invalid start startdate format", nameof(auctionsDTO.EndDate));
            }
            var auction = await _auctionsRepository.GetAuctionsbyID(auctionId);
           if(auction == null)
           {
                return "Auction not Found";
           }
           else
           {
                auction.StartDate = startdate;
                auction.EndDate = endDate;
                auction.Status = auctionsDTO.Status;
                auction.StartingPrice = auctionsDTO.StartingPrice;
                auction.Description = auctionsDTO.Description;
                auction.Approve = auctionsDTO.Approve;
                auction.Title = auctionsDTO.Title;
                auction.Detail = auctionsDTO.Detail;
                await _auctionsRepository.UpdateAuctions(auction);
                return "Success";
           }
        }

        public async Task<bool> UpdateStatusAuction(string auctionId, AuctionStatus newStatus)
        {
            bool statusChanges = false;
            var auction = await _auctionsRepository.GetAuctionsbyID(auctionId);
            if(auction == null)
            {
                return false;
            }
            if(auction.Status == newStatus)
            {
                return false;
            }
           switch (auction.Status)
            {
                case AuctionStatus.Active:
                    if(newStatus == AuctionStatus.Done)
                    {
                        auction.Status = AuctionStatus.Done;
                        statusChanges = true;
                    }else if(newStatus == AuctionStatus.None)
                    {
                        auction.Status = AuctionStatus.None;
                        statusChanges = true;
                    }
                    break;
                default: 
                    return false;  
            }
            if (!statusChanges)
            {
                return false; // Trạng thái mới không hợp lệ dựa trên trạng thái hiện tại
            }
            await _auctionsRepository.UpdateAuctions(auction);
            return true;
        }

        #region Get auction detail include img

        public async Task<AuctionDetailsDTO> GetAuctionDetailsAsync(string auctionId)
        {
            var auction = await _auctionsRepository.GetAuctionWithDetailsAsync(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");
            }

            return new AuctionDetailsDTO
            {
                ID = auction.ID,
                StartDate = auction.StartDate,
                EndDate = auction.EndDate,
                Status = auction.Status,
                StartingPrice = auction.StartingPrice,
                FinalPrice = auction.FinalPrice,
                Description = auction.Description,
                Approve = auction.Approve,
                Title = auction.Title,
                Detail = auction.Detail,
                ProductId = auction.ProductId,
                ProductName = auction.Products.Name,
                ProductDescription = auction.Products.Description,
                Images = auction.Products.Images.Select(i => new Data.Models.ResponseDTO.ImageDTO
                {
                    Id = i.Id,
                    ImgUrl = i.ImgUrl
                }).ToList()
            };
        }
        #endregion

        #region Get all auction with img

        public async Task<List<AuctionDetailsDTO>> GetAuctionsByStatusAndSearchTermAsync(AuctionStatus status, string? searchTerm)
        {
            var auctions = await _auctionsRepository.GetAuctionsByStatusAndSearchTermAsync(status, searchTerm);
            if (searchTerm != null && searchTerm.Length < 3)
            {
                throw new KeyNotFoundException("Search term must be more than 3 character");
            }
            else
            {

                if (auctions == null || !auctions.Any())
                {
                    throw new KeyNotFoundException($"No auctions found with status {status} and search term '{searchTerm}'.");
                }

                return auctions.Select(a => new AuctionDetailsDTO
                {
                    ID = a.ID,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Status = a.Status,
                    StartingPrice = a.StartingPrice,
                    FinalPrice = a.FinalPrice,
                    Description = a.Description,
                    Approve = a.Approve,
                    Title = a.Title,
                    Detail = a.Detail,
                    ProductId = a.ProductId,
                    ProductName = a.Products.Name,
                    ProductDescription = a.Products.Description,
                    Images = a.Products.Images.Select(i => new Data.Models.ResponseDTO.ImageDTO
                    {
                        Id = i.Id,
                        ImgUrl = i.ImgUrl
                    }).ToList()
                }).ToList();
            }
        }
        #endregion
    }
}
