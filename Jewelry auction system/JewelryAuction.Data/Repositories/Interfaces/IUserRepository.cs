using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models.ResponseDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<Users>> GetAllUsers();
        Task<Users> GetUserById(string userID);
        public Users GetUserByIdNoAsync(string userId);
        Task AddUserAsync(Users user);
        Task<Users> GetUserByEmailAsync(string email);
        Task EditUserAsync(Users user);
        Task<string> GetEmailByIdAsync(string id);

    }
}
