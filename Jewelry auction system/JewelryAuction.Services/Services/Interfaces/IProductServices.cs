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
    public interface IProductServices
    {
        Task<List<ProductReponseDTO>> GetAllProduct(string searchterm, string filter);
        Task<GetProductByIdDTO> GetProductbyIdAsync(string productId);
        Task<Products> GetProductsById(string ProductId);
        Task<string> UpdateProduct(string ProdcutID, ProductRequestDTO ProductDTO);
        Task<string> DeleteProduct(string ProductID);
        Task<bool> UpdaterStatusProduct(string productID, ProductStatus newStatus);
        Task<List<ProductsByUserIdDTO>> GetAllProductsByUserIdAsync(string userId);
        Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Requesting();
        Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Accepted();
        Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Denied();
        Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Hold();
        Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Sold();
    }
}
