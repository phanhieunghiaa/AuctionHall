using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories
{
    public class ImgRepository : IImgRepository
    {
        private readonly ApplicationDbContext _context;
        public ImgRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Images> GetImageById(string ProductId)
        {
            return await _context.Images.FirstOrDefaultAsync(sc => sc.ProductId.Equals(ProductId));
        }

        public async Task AddImage(Images images)
        {
            await _context.Images.AddAsync(images);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ImageRequestDTO>> GetAllImagesAsync(string productId)
        {
            return await _context.Images
                                 .Where(sc => sc.ProductId == productId)
                                 .Select(image => new ImageRequestDTO
                                 {
                                     ImgUrl = image.ImgUrl,
                                     ProductId = image.ProductId
                                 })
                                 .ToListAsync();
        }


    }
}
