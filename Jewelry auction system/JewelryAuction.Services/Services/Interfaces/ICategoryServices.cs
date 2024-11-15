using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface ICategoryServices
    {
        Task<List<Categories>> GetAllCategories();
        Task<Categories> GetCategoriesById(string id);
        Task<ServiceResponse<CategoriesDTO>> AddCategories(CategoriesDTO categoriesDTO);
        Task<ServiceResponse<Categories>> DeleteCategories(string id);
        Task AddListCategoryType(CategoryTypeRequestDTO categoryTypeRequestDTO);
    }
}
