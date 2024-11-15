using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductReponseDTO>> GetAllProduct(string searchterm, string filter);
        Task AddProduct(Products products);
        Task<GetProductByIdDTO> GetProductbyIdAsync(string id);
        Task<Products> GetProductsById(string ProductId);
        Task DeleteProduct(string productID);
        Task<bool> UpdateProduct(Products product);
        Task<bool> UpdateStatusProduct(string productId, ProductStatus newStatus);
        Task<List<Products>> GetAllProductsByUserIdAsync(string userId);
        Task<List<Products>> GetAllProductsByStatusAsync(ProductStatus status);
        Task<Products> GetSimpleProductByIdAsync(string id);

    }
}
