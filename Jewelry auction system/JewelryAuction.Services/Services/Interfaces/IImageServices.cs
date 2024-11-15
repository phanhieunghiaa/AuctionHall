using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IImageServices
    {
        Task<Images> GetImageById(string ProductId);
        Task<bool> AddImage(string imgUrl, string productID);
        Task<List<ImageRequestDTO>> GetAllImagesAsync(string productId);
    }
}
