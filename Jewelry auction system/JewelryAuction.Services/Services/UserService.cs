using JewelryAuction.Services.Services.Interfaces;
using JewelryAuction.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Repositories;
using System.Globalization;
using System.Text.RegularExpressions;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Services.Helpers;
using JewelryAuction.Services.Services.Background;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace JewelryAuction.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IEmailServices _emailServices;
        private readonly VerificationCodeManager _verificationCodeManager;
        private readonly ILogger<AuctionCheckService> _logger;
        private readonly IMemoryCache _cache;

        //private readonly ConcurrentDictionary<string, Users> _temporaryUsers = new ConcurrentDictionary<string, Users>();

        public UserService(IUserRepository userRepository, IEmailServices emailServices, 
            VerificationCodeManager verificationCodeManager, ILogger<AuctionCheckService> logger, 
            IMemoryCache memoryCache, IWalletRepository walletRepository)
        {
            _userRepository = userRepository;
            _emailServices = emailServices;
            _verificationCodeManager = verificationCodeManager;
            _logger = logger;
            _cache = memoryCache;
            _walletRepository = walletRepository;
        }
        public async Task<List<Users>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }
        public async Task<Users> GetUserById(string UserID)
        {
            return await _userRepository.GetUserById(UserID);
        }

        public async Task<Users> GetUserByEmail(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        #region Get user profile
        public async Task<UserProfileDTO> GetUserProfile(string Id)
        {
            var user = await _userRepository.GetUserById(Id);
            var userProfileDTO = new UserProfileDTO();
            if (user == null)
            {
                throw new ArgumentException("This user doesn't exist!");
            }
            else
            {
                var wallet = await _walletRepository.GetWalletByUserIdAsync(Id);
                var splitName = await SplitFirstAndLastName(user.FullName);
                userProfileDTO.Id = user.ID;
                userProfileDTO.FullName = user.FullName;
                userProfileDTO.FirstName = splitName.firstName;
                userProfileDTO.LastName = splitName.lastName;
                userProfileDTO.Email = user.Email;
                userProfileDTO.Password = user.Password;
                userProfileDTO.Phone = user.Phone;
                userProfileDTO.CID = user.CID.ToString();
                userProfileDTO.Address = user.Address;
                userProfileDTO.Status = user.Status.Value.ToString();
                userProfileDTO.WalletId = wallet.ID;
            }

            return userProfileDTO;
        }
        #endregion

        #region Register
        public async Task<bool> AddUserAsync(string email, string fullName, string password)
        {
            var isValidEmail = await IsValidEmail(email);
            if (!isValidEmail)
            {
                throw new ArgumentException("Wrong email format"); // Email không hợp lệ
            }

            var existingUser = await _userRepository.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                return false; // Email đã tồn tại
            }


            var verificationCode = GenerateVerificationCode();
            //_logger.LogInformation($"Confirm code is : {verificationCode}");
            var expiration = TimeSpan.FromMinutes(10);
            //_logger.LogInformation($"Time expiration : {expiration}");

            await _verificationCodeManager.SetVerificationCodeAsync(email, verificationCode, expiration);

            await _emailServices.SendEmail_ConfirmCode(email, fullName, verificationCode);


            var user = new Users
            {
                Email = email,
                FullName = fullName,
                Password = password,
                UpdatedBy = "System",
                Role = (Data.Enums.UserRole?)1,
                Status = Data.Enums.UserStatus.Validated,
                CreatedDate = DateTime.Now
            };

            _cache.Set(verificationCode, user);
            //_logger.LogInformation($"User INFOR : {user}");
            //_logger.LogInformation($"Temporary users count in register: {_cache.Get<int>("UserCount")}");
            return true;
        }

        public async Task<bool> ConfirmUserAsync(string email, string code)
        {
            //_logger.LogInformation($"ConfirmUserAsync called with email: {email} and code: {code}");

            if (await _verificationCodeManager.ValidateVerificationCodeAsync(email, code))
            {
                //_logger.LogInformation($"Verification code for {email} is valid.");
                //_logger.LogInformation($"Temporary users count in confirm: {_cache.Get<int>("UserCount")}");

                if (_cache.TryGetValue(code, out Users user))
                {
                    //_logger.LogInformation($"User found in temporary storage: {user}");

                    await _userRepository.AddUserAsync(user);
                    await _verificationCodeManager.RemoveVerificationCodeAsync(email);
                    _cache.Remove(email);
                    return true;
                }
                else
                {
                    _logger.LogWarning($"User not found in temporary storage for email: {email}");
                }
            }
            else
            {
                _logger.LogWarning($"Invalid verification code for email: {email}");
            }  
            return false;
        }
        #endregion

        #region Update
        public async Task<bool> UpdateUserAsync(UpdateUserDTO user, string UserID)
        {
            var existingUser = await _userRepository.GetUserById(UserID);
            if (existingUser == null)
            {
                return false;
            }
            else
            {
                existingUser.FullName = user.FullName;
                if (await IsValidEmail(user.Email)) { existingUser.Email = user.Email; }
                else return false; 
                /*if (user.Password != "")
                {
                    existedUser.Password = HashAndTruncatePassword(user.Password);
                }*/
                existingUser.Password = user.Password;
                existingUser.CID = user.CID;
                existingUser.Phone = user.Phone;
                existingUser.Address = user.Address;
                existingUser.UpdatedBy = "System";
                existingUser.UpdatedDate = DateTime.Now;
            }
            await _userRepository.EditUserAsync(existingUser);
            return true;
        }
        #endregion

        #region Sub code

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private async Task<bool> IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    var idn = new IdnMapping();

                    // Use IdnMapping class to convert Unicode domain names.
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public async Task<Users> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.Password != password)
            {
                return null;
            }

            return user;
        }

        private async Task<(string firstName, string lastName)> SplitFirstAndLastName(string fullName)
        {
            // Tách chuỗi thành các từ dựa trên khoảng trắng và loại bỏ các mục trống
            string[] words = fullName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Lấy từ đầu tiên và từ cuối cùng
            string firstName = words[0];
            string lastName = words[^1];
            Console.WriteLine(firstName + " " + lastName);

            return (firstName, lastName);
        }
        #endregion

    }
}
