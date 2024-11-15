using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IProductCategoryServices
    {
        Task<List<ProductCategoryReponseDTO>> GetAllProductInCateGory(string searchterm, string CategorytypeID);
        Task<string> AddProductInCateGories(string ProductID, string CategoryTypeID);
        Task<ProductCategoryReponseDTO> GetProductCategoryAsync(string ProductID, string CategoryTypeID);
        Task<ServiceResponse<AddProductResponseDTO>> AddProductWithCategoryTypesAsync(ProductWithCategoryTypesDTO productDTO);
        Task<ServiceResponse<AddProductResponseDTO>> UpdateProductWithCategoryType(ProductWithCategoryTypesDTO productDTO, string productId);
    }
}
