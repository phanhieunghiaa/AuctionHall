using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Repositories;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services
{
    public class ImageService : IImageServices
    {
        private readonly IImgRepository _imgRepository;
        private readonly IProductRepository _productRepository;
        public ImageService(IImgRepository imgRepository, IProductRepository productRepository)
        {
            _imgRepository = imgRepository;
            _productRepository = productRepository;
        }
        public async Task<Images> GetImageById(string ProductId)
        {
                if (ProductId == null)
                {
                    throw new ArgumentNullException("Product ID not found, can't find image");
                }
                return await _imgRepository.GetImageById(ProductId);           
        }

        public async Task<bool> AddImage(string imgUrl, string productID)
        {
            var existingProduct = await _productRepository.GetProductsById(productID);
            if (existingProduct == null)
            {
                return false;
            }
            try
            {
                Images newImage = new Images()
                {
                    ImgUrl = imgUrl,
                    ProductId = productID
                };
                await _imgRepository.AddImage(newImage);
                return true;
            }
            catch { Exception ex; return false; }
        }

        #region Get all img by productId

        public async Task<List<ImageRequestDTO>> GetAllImagesAsync(string productId)
        {
            var imageDTO = new List<ImageRequestDTO>();
            var product = await _productRepository.GetProductsById(productId);
            if (product == null)
            {
                throw new KeyNotFoundException("Cannot find productId");
            };

            var imgs = await _imgRepository.GetAllImagesAsync(productId);
            if (imgs == null)
            {
                throw new KeyNotFoundException("Cannot find any images by this productId");
            };
            return imgs; 
        }
        #endregion
    }
}
