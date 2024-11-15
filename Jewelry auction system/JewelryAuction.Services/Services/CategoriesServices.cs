using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Data.Repositories.Interfaces;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services
{
    public class CategoriesServices: ICategoryServices
    {
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly ApplicationDbContext _context;

        public CategoriesServices(ICategoriesRepository categoriesRepository, ApplicationDbContext context)
        {
            _categoriesRepository = categoriesRepository;
            _context = context;
        }

        public async Task<List<Categories>> GetAllCategories()
        {
            return await _categoriesRepository.GetAllCategories();
        }

        public async Task<Categories> GetCategoriesById(string id)
        {
            return  await _categoriesRepository.GetCategoriesById(id);
        }

        public async Task<ServiceResponse<CategoriesDTO>> AddCategories(CategoriesDTO categoriesDTO)
        {
            if(categoriesDTO == null)
            {
                return ServiceResponse<CategoriesDTO>.ErrorResponse("Categories Not Found");
            }
            if (string.IsNullOrWhiteSpace(categoriesDTO.Category) || categoriesDTO.Category == "string")
            {
                return ServiceResponse<CategoriesDTO>.ErrorResponse("Product name is required");
            }
            Categories categories = new Categories()
            {
                ID = Guid.NewGuid().ToString(),
                //Type = categoriesDTO.Type,
                //Color = categoriesDTO.Color,
                //Material = categoriesDTO.Material,
                Category = categoriesDTO.Category,
            };
            await _categoriesRepository.AddCategories(categories);
            return ServiceResponse<CategoriesDTO>.SuccessResponseWithMessage(categoriesDTO, "Category added successfully.");
        }

        public async Task<ServiceResponse<Categories>> DeleteCategories(string id)
        {
            var categories = await _categoriesRepository.GetCategoriesById(id);
            if (categories == null)
            {
                return ServiceResponse<Categories>.ErrorResponse("Category not found.");
            }
            else
            {
               await _categoriesRepository.DeleteCategories(id);
                return ServiceResponse<Categories>.SuccessResponseWithMessage(categories, "Category delete successfully.");
            }
        }

        public async Task AddListCategoryType(CategoryTypeRequestDTO categoryTypeRequestDTO)
        {
            var category = await _categoriesRepository.GetCategoriesById(categoryTypeRequestDTO.CategoryId);

            if(category == null)
            {
                throw new ArgumentException("Category not found with ID: " + categoryTypeRequestDTO.CategoryId);
            }

            foreach(var typeDetail in categoryTypeRequestDTO.CategoryTypes)
            {
                var categoryType = new CategoryType
                {
                    CategoryId = categoryTypeRequestDTO.CategoryId,
                    CategoryTypes = typeDetail.CategoryTypes,
                };
                _context.CategoryTypes.Add(categoryType);
            }
            await _context.SaveChangesAsync();
        }
    }
}
