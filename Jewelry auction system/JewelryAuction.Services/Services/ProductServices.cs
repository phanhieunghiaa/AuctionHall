using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Data.Repositories;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace JewelryAuction.Services.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IUserRepository _userRepository;
        private readonly ICategoriesTypeRepository _categoriesTypeRepository;
        private readonly IImgRepository _imgRepository;

        public ProductServices(IProductRepository productRepository, ApplicationDbContext applicationDbContext, IUserRepository userRepository, ICategoriesTypeRepository categoriesTypeRepository, IImgRepository imgRepository)
        {
            _productRepository = productRepository;
            _applicationDbContext = applicationDbContext;
            _userRepository = userRepository;
            _categoriesTypeRepository = categoriesTypeRepository;
            _imgRepository = imgRepository;
        }

        #region Get All Product anh searching 
        public async Task<List<ProductReponseDTO>> GetAllProduct(string searchterm, string? filter)
        {
            try
            {

                return await _productRepository.GetAllProduct(searchterm, filter);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region Get Product by Id with List Type
        public async Task<GetProductByIdDTO> GetProductbyIdAsync(string productId)
        {
            return await _productRepository.GetProductbyIdAsync(productId);
        }

        #endregion

        #region Get Product by Id
        public async Task<Products> GetProductsById(string ProductId)
        {
            try
            {
                if (ProductId == null)
                {

                    throw new ArgumentNullException("Products not found");
                }
                return await _productRepository.GetProductsById(ProductId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        #endregion

        #region Add Product
        public async Task<ServiceResponse<AddProductResponseDTO>> AddProduct(ProductDTO productDTO)
        {
            try
            {
                if (productDTO == null)
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("Product information cannot be blank");
                }

                if (string.IsNullOrWhiteSpace(productDTO.Name) || productDTO.Name == "string")
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("Product name is required");
                }
                var user = await _userRepository.GetUserById(productDTO.UserId);
                if (user == null)
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("User Not found");
                }
                Products newProduct = new Products()
                {
                    Name = productDTO.Name,
                    Status = 0,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    Price = productDTO.Price,
                    Description = productDTO.Description,
                    UpdatedBy = user.FullName,
                    UserId = productDTO.UserId,
                };


                await _productRepository.AddProduct(newProduct);
                // Handling Images if specified
                if (productDTO.ImageUrls != null && productDTO.ImageUrls.Count > 0)
                {
                    foreach (var url in productDTO.ImageUrls)
                    {
                        var image = new Images
                        {
                            ImgUrl = url,
                            ProductId = newProduct.ID
                        };
                        await _imgRepository.AddImage(image);
                    }
                }
                //
                return ServiceResponse<AddProductResponseDTO>.SuccessResponse(new AddProductResponseDTO { ProductID = newProduct.ID });
            }
            catch (Exception ex)
            {
                return ServiceResponse<AddProductResponseDTO>.ErrorResponse($"An error occurred while adding the product: {ex.Message}");
            }
        }
        #endregion

        #region Update Product            
        public async Task<string> UpdateProduct(string ProductID, ProductRequestDTO ProductDTO)
        {
            string[] dateFormats = { "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy" };
            DateTime date;
            if (!DateTime.TryParseExact(ProductDTO.UpdatedDate, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                throw new ArgumentException("Invalid start date format", nameof(ProductDTO.UpdatedDate));
            }
            var existingProduct = await _productRepository.GetProductsById(ProductID);
            if (existingProduct == null)
            {
                return "Product not found";
            }
            else
            {
                existingProduct.Name = ProductDTO.Name;
                existingProduct.Status = ProductDTO.Status;
                existingProduct.CreatedDate = ProductDTO.CreatedDate;
                existingProduct.UpdatedDate = date;
                existingProduct.UpdatedBy = ProductDTO.UpdatedBy;
                var result = await _productRepository.UpdateProduct(existingProduct);
                return result ? "Update Successful" : "Update failed";
            }
        }
        #endregion

        #region Update Status
        public async Task<bool> UpdaterStatusProduct(string productID, ProductStatus newStatus)
        {
           return await _productRepository.UpdateStatusProduct(productID, newStatus);
        }
        #endregion

        #region Delete Product
        public async Task<string> DeleteProduct(string ProductID)
        {
            Products products = await _productRepository.GetProductsById(ProductID);
            if (products == null)
            {
                return "Product not found";
            }
            await _productRepository.DeleteProduct(ProductID);
            return "Delete Success";
        }
        #endregion

        #region Get product by userId
        public async Task<List<ProductsByUserIdDTO>> GetAllProductsByUserIdAsync(string userId)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Cannot find this userId");
            }
            var products = await _productRepository.GetAllProductsByUserIdAsync(userId);
            if (products == null)
            {
                throw new KeyNotFoundException("Cannot find any product with this userId");
            }         
            return products.Select(p => new ProductsByUserIdDTO
            {
                Id = p.ID,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Status = Enum.GetName(typeof(ProductStatus), p.Status),
                CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedDate = p.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedBy = p.UpdatedBy
            }).ToList();
        }
        #endregion

        #region Get product by status
        //Status = Requesting
        public async Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Requesting()
        {
            ProductStatus status = ProductStatus.Requesting;
            var products = await _productRepository.GetAllProductsByStatusAsync(status);
            if (products == null || !products.Any())
            {
                throw new KeyNotFoundException($"No products found with status {status}.");
            }

            return products.Select(p => new ProductsByStatusDTO
            {
                ID = p.ID,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Status = Enum.GetName(typeof(ProductStatus), p.Status),
                CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedDate = p.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedBy = p.UpdatedBy,
                UserId = p.UserId
            }).ToList();
        }

        //Status = Accepted
        public async Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Accepted()
        {
            ProductStatus status = ProductStatus.Accepted;
            var products = await _productRepository.GetAllProductsByStatusAsync(status);
            if (products == null || !products.Any())
            {
                throw new KeyNotFoundException($"No products found with status {status}.");
            }

            return products.Select(p => new ProductsByStatusDTO
            {
                ID = p.ID,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Status = Enum.GetName(typeof(ProductStatus), p.Status),
                CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedDate = p.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedBy = p.UpdatedBy,
                UserId = p.UserId
            }).ToList();
        }

        //Status = Denied
        public async Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Denied()
        {
            ProductStatus status = ProductStatus.Denied;
            var products = await _productRepository.GetAllProductsByStatusAsync(status);
            if (products == null || !products.Any())
            {
                throw new KeyNotFoundException($"No products found with status {status}.");
            }

            return products.Select(p => new ProductsByStatusDTO
            {
                ID = p.ID,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Status = Enum.GetName(typeof(ProductStatus), p.Status),
                CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedDate = p.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedBy = p.UpdatedBy,
                UserId = p.UserId
            }).ToList();
        }

        //Status = Hold
        public async Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Hold()
        {
            ProductStatus status = ProductStatus.Hold;
            var products = await _productRepository.GetAllProductsByStatusAsync(status);
            if (products == null || !products.Any())
            {
                throw new KeyNotFoundException($"No products found with status {status}.");
            }

            return products.Select(p => new ProductsByStatusDTO
            {
                ID = p.ID,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Status = Enum.GetName(typeof(ProductStatus), p.Status),
                CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedDate = p.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedBy = p.UpdatedBy,
                UserId = p.UserId
            }).ToList();
        }

        //Status = Sold
        public async Task<List<ProductsByStatusDTO>> GetAllProductsByStatus_Sold()
        {
            ProductStatus status = ProductStatus.Sold;
            var products = await _productRepository.GetAllProductsByStatusAsync(status);
            if (products == null || !products.Any())
            {
                throw new KeyNotFoundException($"No products found with status {status}.");
            }

            return products.Select(p => new ProductsByStatusDTO
            {
                ID = p.ID,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                Status = Enum.GetName(typeof(ProductStatus), p.Status),
                CreatedDate = p.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedDate = p.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss"),
                UpdatedBy = p.UpdatedBy,
                UserId = p.UserId
            }).ToList();
        }
        #endregion

    }
}