using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories
{
    public class ProductsRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Get All product 
        public async Task<List<ProductReponseDTO>> GetAllProduct(string searchterm, string? filter)
        {
            var query = _context.Products
                .Include(p => p.Users)
                .Include(p => p.Images)
                .Include(p => p.ProductCategories)  // Include ProductCategory relationship
                .ThenInclude(pc => pc.CategoryType)  // Include CategoryType relationship
                .AsQueryable();
            if (!string.IsNullOrEmpty(searchterm))
            {
                query = query.Where(sc => sc.Name.Contains(searchterm));
            }
            query = ApplyFilter(query, filter);
            var products = await query.Select(p => new ProductReponseDTO
            {
                ID = p.ID,
                Name = p.Name,
                CreatedDate = p.CreatedDate == DateTime.MinValue ? null : p.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedDate = p.UpdatedDate.HasValue ? p.UpdatedDate.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                UpdatedBy = p.UpdatedBy,
                UserId = p.UserId,
                Price = p.Price,
                Description = p.Description,         
                Role = p.Users.Role,
                ImgUrl = p.Images.Select(img => img.ImgUrl).ToList(),
                CategoryTypeNames = p.ProductCategories.Select(pc => pc.CategoryType.CategoryTypes).ToList()  // Get CategoryType names
            }).ToListAsync();

            return products;
        }
        #endregion

        #region Filter
        private IQueryable<Products> ApplyFilter(IQueryable<Products> query, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter.ToLower().Trim()) // filter about role
                {
                    case "member":
                        query = query.Where(p => p.Users.Role == Enums.UserRole.Member);
                        break;
                    case "seller":
                        query = query.Where(p => p.Users.Role == Enums.UserRole.Seller);
                        break;
                    case "staff":
                        query = query.Where(p => p.Users.Role == Enums.UserRole.Staff);
                        break;
                    case "manager":
                        query = query.Where(p => p.Users.Role == Enums.UserRole.Manager);
                        break;
                    case "admin":
                        query = query.Where(p => p.Users.Role == Enums.UserRole.Admin);
                        break;
                }
                switch (filter.ToLower().Trim()) // filter about the Type 
                {
                    case "red":
                        query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryType.CategoryTypes == "Red".ToLower()));
                        break;
                    case "blue":
                        query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryType.CategoryTypes == "blue".ToLower()));
                        break;
                    case "gold":
                        query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryType.CategoryTypes == "gold".ToLower()));
                        break;
                    case "diamond":
                        query = query.Where(p => p.ProductCategories.Any(pc => pc.CategoryType.CategoryTypes == "diamond".ToLower()));
                        break;
                }

            }
            return query;
        }
        #endregion

        #region Get Product by Id (list image, type)
        public async Task<GetProductByIdDTO> GetProductbyIdAsync(string id)
        {
            var product = await _context.Products
            .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.CategoryType)
                    .ThenInclude(ct => ct.Category)
            .Include(p => p.Images)
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p => p.ID == id);

            if (product == null)
            {
                return null;
            }

            // Mapping manually
            var productDto = new GetProductByIdDTO()
            {
                ID = product.ID,
                Name = product.Name,
                CreatedDate = product.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedDate = product.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                Price = product.Price,
                Description = product.Description,
                Status = Enum.GetName(typeof(ProductStatus), product.Status),
                UpdatedBy = product.UpdatedBy,
                UserId = product.UserId,
                Role = product.Users.Role,
                ImgUrl = product.Images.Select(img => img.ImgUrl).ToList(),
                CategoryTypeNames =product.ProductCategories.GroupBy(pc => pc.CategoryType.Category.Category).
                        ToDictionary(g => g.Key, g => string.Join(", ", g.Select(pc => pc.CategoryType.CategoryTypes)))
            };

            return productDto;
        }
        #endregion

        #region Get Product by Id (normal)
        public async Task<Products> GetProductsById(string ProductId)
        {
            return await _context.Products.FirstOrDefaultAsync(sc => sc.ID.Equals(ProductId));
        }
        #endregion

        #region Add Product
        public async Task AddProduct(Products products)
        {
            _context.Products.Add(products);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Update Product
        public async Task<bool> UpdateProduct(Products product)
        {
            _context.Products.Update(product);
            return await _context.SaveChangesAsync() > 0;
        }
        #endregion

        #region Update Status Product
        public async Task<bool> UpdateStatusProduct(string productId, ProductStatus newStatus)
        {
            var product = await GetProductsById(productId);
            if (product == null)
            {
                return false;
            }
            if (product.Status == newStatus)
            {
                return false; // Trạng thái mới trùng với trạng thái hiện tại, không cần cập nhật
            }
            bool statusChangeAllowed = false;
            switch (product.Status)
            {
                case ProductStatus.New:
                    if (newStatus == ProductStatus.Requesting)
                    {
                        product.Status = ProductStatus.Requesting;
                        statusChangeAllowed = true;
                    }
                    break;
                case ProductStatus.Requesting:
                    if (newStatus == ProductStatus.Accepted || newStatus == ProductStatus.Denied)
                    {
                        product.Status = newStatus;
                        statusChangeAllowed = true;
                    }
                    break;
                case ProductStatus.Accepted:
                    if (newStatus == ProductStatus.Sold || newStatus == ProductStatus.Hold)
                    {
                        product.Status = newStatus;
                        statusChangeAllowed = true;
                    }
                    break;
                default:
                    return false;
            }
            if (!statusChangeAllowed)
            {
                return false; // Trạng thái mới không hợp lệ dựa trên trạng thái hiện tại
            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }


        #endregion

        #region Delete Product
        public async Task DeleteProduct(string productID)
        {
            Products products = await GetProductsById(productID);
            if (products != null)
            {
                _context.Products.Remove(products);
                await _context.SaveChangesAsync();
            }
        }
        #endregion

        #region Get all Product with userId

        public async Task<List<Products>> GetAllProductsByUserIdAsync(string userId)
        {
            return await _context.Set<Products>()
                                 .Where(p => p.UserId == userId)
                                 .ToListAsync();
        }
        #endregion

        #region Get all product by status

        public async Task<List<Products>> GetAllProductsByStatusAsync(ProductStatus status)
        {
            return await _context.Products.Where(p => p.Status == status).ToListAsync();
        }
        #endregion

        #region Sub code

        public async Task<Products> GetSimpleProductByIdAsync(string id)
        {
            return await _context.Products.Include(p => p.Images).Include(p => p.Auctions).Include(p => p.ProductCategories).FirstOrDefaultAsync(p => p.ID == id);
        }
        #endregion

    }
}
