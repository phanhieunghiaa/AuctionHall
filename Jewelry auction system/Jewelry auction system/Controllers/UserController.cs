using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Services.Helpers;
using JewelryAuction.Services.Services;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWalletServices _walletServices;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly IConfiguration _configuration;


        public UserController(IUserService userService, JwtTokenHelper jwtTokenHelper, IConfiguration configuration, IWalletServices walletServices)
        {
            _userService = userService;
            _jwtTokenHelper = jwtTokenHelper;
            _configuration = configuration;
            _walletServices = walletServices;
        }

        [Authorize(Policy = "AllRole")]
        [HttpGet]
        [Route("GetUserByID")]
        public async Task<IActionResult> GetUserByID([Required] String id)
        {
            try
            {
                var result = await _userService.GetUserProfile(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Trả về thông báo lỗi định dạng email
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred."); // Trả về lỗi chung
            }
        }


        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var user = await _userService.GetAllUsers();
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [Authorize(Policy = "Member")]
        [HttpPatch]
        [Route("UpdateUserByID")]
        public async Task<IActionResult> UpdateUserByID([FromQuery]String id, [FromBody] UpdateUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _userService.UpdateUserAsync(userDTO, id);
                return Ok(result ? "Update Successful" : "Update failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }

        }

        [HttpPost("User register")]
        public async Task<IActionResult> AddUser([FromBody] RegisterDTO userDto)
        {
            try
            {
                var result = await _userService.AddUserAsync(userDto.Email, userDto.FullName, userDto.Password);
                if (!result)
                {
                    return BadRequest("Email already exists.");
                }

                return Ok("User created successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Trả về thông báo lỗi định dạng email
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred."); // Trả về lỗi chung
            }
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm(string email, string code)
        {
            var result = await _userService.ConfirmUserAsync(email, code);
            if (result)
            {
                var user = await _userService.GetUserByEmail(email);
                await _walletServices.AddWallet(user.ID);
                return Ok(new { message = "User confirmed and registered successfully." });
            }
            return BadRequest(new { message = "Invalid verification code or email." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userService.AuthenticateAsync(model.Email, model.Password);
            if (user == null)
            {
                return Unauthorized();
            }

            // Tạo token JWT và trả về cho người dùng
            var token = _jwtTokenHelper.GenerateJwtToken(user);
            return Ok(new { token });
        }

    }
}
