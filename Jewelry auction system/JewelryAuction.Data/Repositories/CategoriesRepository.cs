using Jewelry_auction_system.Data;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories
{
    public class CategoriesRepository: ICategoriesRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Categories>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        } 

        public async Task<Categories> GetCategoriesById(string id)
        {
            return await _context.Categories.FirstOrDefaultAsync(sc => sc.ID.Equals(id));
        }

        public async Task AddCategories(Categories categories)
        {
            _context.Categories.Add(categories);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategories(Categories categories)
        {
            _context.Categories.Update(categories);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategories(string id)
        {
            Categories categories = await GetCategoriesById(id);
            if (categories != null)
            {
                _context.Categories.Remove(categories);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Categories not found");
            }
        }
    }
}
