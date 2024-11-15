using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
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
    public class CategoryTypeServices: ICategoryTypeServices
    {
        private readonly ICategoriesTypeRepository _categoriesTypeRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly ApplicationDbContext _context;

        public CategoryTypeServices(ICategoriesTypeRepository categoriesTypeRepository, ICategoriesRepository categoriesRepository, ApplicationDbContext context)
        {
            _categoriesTypeRepository = categoriesTypeRepository;
            _categoriesRepository = categoriesRepository;
            _context = context;
        }

        public async Task<List<CategoryType>> GetAllType()
        {
            return await _categoriesTypeRepository.GetAllType();
        }

        public async Task<ServiceResponse<CategoryTypeDTO>> AddType(CategoryTypeDTO categoryTypeDTO)
        {
            try
            {
            if (categoryTypeDTO == null)
            {
                return ServiceResponse<CategoryTypeDTO>.ErrorResponse("Type not found");
            }
            var category = await _categoriesRepository.GetCategoriesById(categoryTypeDTO.CategoryId);
            if (category == null)
            {
                return ServiceResponse<CategoryTypeDTO>.ErrorResponse("Category not found");
            }
            CategoryType categoryType = new CategoryType()
            {
                ID = Guid.NewGuid().ToString(),
                CategoryTypes = categoryTypeDTO.CategoryTypes,
                CategoryId = categoryTypeDTO.CategoryId,
                //Category =    category.Category,
            };
            await _categoriesTypeRepository.AddType(categoryType);
            return ServiceResponse<CategoryTypeDTO>.SuccessResponseWithMessage(categoryTypeDTO ,"Add Type SuccessFully");
            }catch (Exception ex)
            {
                return ServiceResponse<CategoryTypeDTO>.ErrorResponse($"An error occurred while adding the categoty Type: {ex.Message}");
            }
        }

       
    }
}
