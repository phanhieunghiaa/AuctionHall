using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Data.Entity;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace JewelryAuction.Services.Services
{
    public class ProductCategoryServices: IProductCategoryServices
    {
        private readonly IProductCategoryRepository _repository;
        private readonly ICategoriesTypeRepository _categoriesTypeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IImageServices _imageService;
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        public ProductCategoryServices(IProductCategoryRepository repository,ICategoriesTypeRepository categoriesTypeRepository, IProductRepository productRepository, IImageServices imageService, ApplicationDbContext context, IUserRepository userRepository)
        {
            _repository = repository;
            _categoriesTypeRepository = categoriesTypeRepository;
            _productRepository = productRepository;
            _imageService = imageService;
            _context = context;
            _userRepository = userRepository;
        }

        #region Get All Product  with List Category
        public async Task<List<ProductCategoryReponseDTO>> GetAllProductInCateGory(string searchterm, string CategorytypeID)
        {
            List<ProductCategoryReponseDTO> productCategoryReponseDTOs = new List<ProductCategoryReponseDTO>();
            var ProductsInCategory = await _repository.GetAllProductCategory(searchterm, CategorytypeID);
            CategoryType type = await _categoriesTypeRepository.GetAllTypeById(CategorytypeID);
            if (type == null)
            {
                throw new KeyNotFoundException($"Category {CategorytypeID} is not excited system");
            }
            foreach (var item in ProductsInCategory)
            {
                var product = await _productRepository.GetProductsById(item.ProductId);
                var newProduct = new ProductCategoryReponseDTO()
                {
                    ID = item.ID,
                    Name = product.Name,
                    Status = product.Status,
                    CreatedDate = product.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss") ?? "No Information",
                    UpdatedDate = product.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss") ?? "No Information",
                    UpdatedBy = product.UpdatedBy, 
                    Price = product.Price,
                    Description = product.Description,
                    ProductId = item.ProductId,
                    CategoryId = item.CategoryTypeId,

                };
                productCategoryReponseDTOs.Add(newProduct);

            }
            return productCategoryReponseDTOs;
        }
        #endregion

        #region AddProductCateGories
        public async Task<string> AddProductInCateGories(string ProductID, string CategoryTypeID)
        {
            Products product = await _productRepository.GetProductsById(ProductID);
            CategoryType type = await _categoriesTypeRepository.GetAllTypeById(CategoryTypeID);
            ProductCategory productCategory = await _repository.GetProductInCategory(ProductID, CategoryTypeID);
            Images images = await _imageService.GetImageById(ProductID);
            if (product == null)
            {
                return "Product ID: " + ProductID + " is not existed on system";
            }
            if (type == null)
            {
                return "Categories ID: " + CategoryTypeID + " is not existed on system";
            }
            if (productCategory != null)
            {
                return "Product ID: " + ProductID + " is existed in Category";
            }
            if (product != null && type != null && productCategory == null)
            {
                await _repository.AddProductCateGory(new ProductCategory()
                {
                    ID = Guid.NewGuid().ToString(),
                    ProductId = ProductID,
                    CategoryTypeId = CategoryTypeID,
                    
                });
            }
            return "Success";
        }
        #endregion

        #region Add Product With Category Types Async
        public async Task<ServiceResponse<AddProductResponseDTO>> AddProductWithCategoryTypesAsync(ProductWithCategoryTypesDTO productDTO)
        {
            try
            {
                var user = await _userRepository.GetUserById(productDTO.UserId);
                if(user == null)
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("User not found");
                }
                if (string.IsNullOrWhiteSpace(productDTO.Name) || productDTO.Name == "string")
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("Product name is required");
                }
                if (string.IsNullOrWhiteSpace(productDTO.Description) || productDTO.Description == "string")
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("Product description is required");
                }
                if (string.IsNullOrWhiteSpace(productDTO.Price) || productDTO.Price == "string")
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("Product price is required");
                }
                var product = new Products
                {
                    ID = Guid.NewGuid().ToString(),
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    Price = productDTO.Price,
                    CreatedDate = DateTime.Now,
                    Status = Data.Enums.ProductStatus.Requesting,
                    UpdatedBy = user.FullName,
                    UserId = productDTO.UserId,
                };
                await _productRepository.AddProduct(product);
             
                foreach (var categoryId in productDTO.CategoryTypeIds)
                {
                    var productCategory = new ProductCategory()
                    { 

                        ID = Guid.NewGuid().ToString(),
                        ProductId = product.ID,
                        CategoryTypeId = categoryId,
                    };
                    if (string.IsNullOrWhiteSpace(productCategory.CategoryTypeId) || productCategory.CategoryTypeId == "string")
                    {
                        return ServiceResponse<AddProductResponseDTO>.ErrorResponse("CategoryId is required");
                    }
                    await _repository.AddProductCateGory(productCategory);
                }
                
                return ServiceResponse<AddProductResponseDTO>.SuccessResponse(new AddProductResponseDTO
                {
                    ProductID = product.ID,
                });
            }
            catch (Exception ex)
            {
                return ServiceResponse<AddProductResponseDTO>.ErrorResponse("Error adding product: " + ex.Message);
            }
        }
        #endregion 

        #region Get Product Category Async
        public async Task<ProductCategoryReponseDTO> GetProductCategoryAsync(string ProductID, string CategoryTypeID)
        {

            var productsInCategory = await _repository.GetProductInCategory(ProductID, CategoryTypeID);
            Products products = await _productRepository.GetProductsById(ProductID);
            CategoryType type = await _categoriesTypeRepository.GetAllTypeById(CategoryTypeID);
            if (type == null)
            {
                throw new KeyNotFoundException($"Category {CategoryTypeID} is not excited system");
            }
            if (type == null)
            {
                throw new KeyNotFoundException($"Product {ProductID} is not excited system");
            }
            if (productsInCategory == null)
            {
                throw new KeyNotFoundException($"Category {ProductID} is not have category");
            }
            var newProduct = new ProductCategoryReponseDTO()
            {
                ID = productsInCategory.ID,
                Name = products.Name,
                Status = products.Status,
                CreatedDate = products.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss") ?? "No Information",
                UpdatedDate = products.UpdatedDate?.ToString("dd/MM/yyyy HH:mm:ss") ?? "No Information",
                UpdatedBy = products.UpdatedBy,         
                Price = products.Price,
                Description = products.Description,
                ProductId = productsInCategory.ProductId,
                CategoryId = productsInCategory.CategoryTypeId,              
            };
            return newProduct;
        }
        #endregion

        #region Update Product with list Category type
        public async Task<ServiceResponse<AddProductResponseDTO>> UpdateProductWithCategoryType(ProductWithCategoryTypesDTO productDTO, string productId)
        {
            try
            {
                var user = await _userRepository.GetUserById(productDTO.UserId);
                if (user == null)
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("User not found");
                }
                var product = await _productRepository.GetProductsById(productId);
                if (product == null)
                {
                    return ServiceResponse<AddProductResponseDTO>.ErrorResponse("Product not found");
                }
                product.Name = productDTO.Name;
                product.Description = productDTO.Description;
                product.Price = productDTO.Price;
                product.UpdatedDate = DateTime.Now;
                product.UpdatedBy = user.FullName;

                await _productRepository.UpdateProduct(product);
                var existingProductCategories = await _repository.GetProductCategoriesByProductId(productId);
                foreach (var existingCategory in existingProductCategories)
                {
                    await _repository.RemoveProductCategory(existingCategory);
                }

                foreach (var categoryId in productDTO.CategoryTypeIds)
                {
                    var productCategory = new ProductCategory()
                    {
                        ID = Guid.NewGuid().ToString(),
                        ProductId = product.ID,
                        CategoryTypeId = categoryId,
                    };
                    await _repository.AddProductCateGory(productCategory);
                }

                return ServiceResponse<AddProductResponseDTO>.SuccessResponse(new AddProductResponseDTO
                {
                    ProductID = product.ID,
                });
            }
            catch (Exception ex)
            {
                return ServiceResponse<AddProductResponseDTO>.ErrorResponse("Error adding product: " + ex.Message);
            }
        }
        #endregion

    }
}
