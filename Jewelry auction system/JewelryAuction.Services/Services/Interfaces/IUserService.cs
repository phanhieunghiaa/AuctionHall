using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelryAuction.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<Users>> GetAllUsers();
        Task<Users> GetUserById(string UserID);
        Task<bool> AddUserAsync(string email, string fullName, string password);
        Task<bool> UpdateUserAsync(UpdateUserDTO user, string UserID);
        Task<Users> AuthenticateAsync(string email, string password);
        Task<UserProfileDTO> GetUserProfile(string Id);
        Task<bool> ConfirmUserAsync(string email, string code);
        Task<Users> GetUserByEmail(string email);
    }
}
