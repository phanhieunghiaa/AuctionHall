using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories
{
    public class ProductCategoryRepository: IProductCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductCategory>> GetAllProductCategory(string searchterm, string CategoryTypeID)
        {
            var query = _context.ProductCategory.AsQueryable();
            if (searchterm != null)
            {
                query = query.Include(o => o.CategoryType).Where(sc => sc.CategoryTypeId == CategoryTypeID && sc.CategoryType.CategoryTypes.Equals(searchterm));
                return await query.ToListAsync();
            }
            else
            {
                return await query.Where(sc => sc.CategoryTypeId == CategoryTypeID).ToListAsync();
            }

        }

        public async Task<ProductCategory> GetProductInCategory(string productId, string categoryTypeId)
        {
            return await _context.ProductCategory.FirstOrDefaultAsync(sc => sc.ProductId == productId && sc.CategoryTypeId == categoryTypeId);
        }

        public async Task AddProductCateGory(ProductCategory productCategory)
        {
            _context.ProductCategory.Add(productCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UppdateProductCategory(ProductCategory productCategory)
        {
            _context.ProductCategory.Update(productCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductCategory(string ProductId, string CategoryTypeId)
        {
            var productCategory = GetProductInCategory(ProductId, CategoryTypeId);
            if (productCategory != null)
            {
                _context.Remove(productCategory);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<IEnumerable<ProductCategory>> GetProductCategoriesByProductId(string productId)
        {
            return await _context.ProductCategory
                .Where(pc => pc.ProductId == productId)
                .ToListAsync();
        }
        public async Task RemoveProductCategory(ProductCategory productCategory)
        {
            _context.ProductCategory.Remove(productCategory);
            await _context.SaveChangesAsync();
        }
    }
}
