using Jewelry_auction_system.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface IProductCategoryRepository
    {
        Task DeleteProductCategory(string ProductId, string CategoryTypeId);
        Task UppdateProductCategory(ProductCategory productCategory);
        Task AddProductCateGory(ProductCategory productCategory);
        Task<ProductCategory> GetProductInCategory(string productId, string categoryTypeId);
        Task<List<ProductCategory>> GetAllProductCategory(string searchterm, string CategoryTypeID);

        Task RemoveProductCategory(ProductCategory productCategory);
        Task<IEnumerable<ProductCategory>> GetProductCategoriesByProductId(string productId);
    }
}
