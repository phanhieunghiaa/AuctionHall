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
    public class UserRepository : IUserRepository
    {
        
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Users>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<Users> GetUserById(string UserID)
        {
            var temp = await _context.Users.FirstOrDefaultAsync(sc => sc.ID.Equals(UserID));
            return temp;
        }

        public Users GetUserByIdNoAsync(string userId)
        {
            var temp = _context.Users.FirstOrDefault(sc => sc.ID.Equals(userId));
            return temp;
        }

        #region Register
        public async Task AddUserAsync(Users user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task<Users> GetUserByEmailAsync(string email)
        {
            var temp = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return temp;
        }
        #endregion

        #region Edit
        public async Task EditUserAsync(Users user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Sub code

        public async Task<string> GetEmailByIdAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ID == id);
            return user?.Email;
        }
        #endregion
    }
}
