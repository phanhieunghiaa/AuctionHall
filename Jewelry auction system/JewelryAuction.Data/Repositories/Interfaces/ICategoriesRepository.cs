using Jewelry_auction_system.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface ICategoriesRepository
    {
        Task<List<Categories>> GetAllCategories();
        Task<Categories> GetCategoriesById(string id);
        Task AddCategories(Categories categories);
        Task UpdateCategories(Categories categories);
        Task DeleteCategories(string id);
    }
}
