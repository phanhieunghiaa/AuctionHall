using JewelryAuction.Data.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface ICategoriesTypeRepository
    {
        Task<List<CategoryType>> GetAllType();
        Task<CategoryType> GetAllTypeById(string id);
        Task AddType(CategoryType categoryType);
        Task UpdateType(CategoryType categoryType);
        Task DeleteType(string id);
    }
}
