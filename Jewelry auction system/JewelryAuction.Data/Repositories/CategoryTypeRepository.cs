using Jewelry_auction_system.Data;
using JewelryAuction.Data.Data.Entity;
using JewelryAuction.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories
{
    public class CategoryTypeRepository: ICategoriesTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryType>> GetAllType()
        {
            return await _context.CategoryTypes.ToListAsync();
        }

        public async Task<CategoryType> GetAllTypeById(string id)
        {
            return await _context.CategoryTypes.FirstOrDefaultAsync(sc => sc.ID == id);
        }

        public async Task AddType(CategoryType categoryType)
        {
            _context.CategoryTypes.Add(categoryType);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateType(CategoryType categoryType)
        {
            _context.CategoryTypes.Update(categoryType);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteType(string id)
        {
            var type = GetAllTypeById(id);
            if(type != null)
            {
               _context.Remove(type);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Type not found");
            }
        }
    }
}
