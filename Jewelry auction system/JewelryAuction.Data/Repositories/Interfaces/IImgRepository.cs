using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models.ReqpuestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface IImgRepository
    {
        Task<Images> GetImageById(string ProductId);
        Task AddImage(Images images);
        Task<List<ImageRequestDTO>> GetAllImagesAsync(string productId);
    }
}
