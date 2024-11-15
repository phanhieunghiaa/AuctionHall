using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletControllers : ControllerBase
    {
        private readonly IWalletServices _walletServices;

        public WalletControllers(IWalletServices walletServices)
        {
            _walletServices = walletServices;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWallets()
        {
            var result = await _walletServices.GetAllWallets();
            return Ok(result);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetWalletById(string? walletId)
        {
            var wallet = await _walletServices.GetWalletById(walletId);
            if (wallet == null)
            {
                return NotFound();
            }
            return Ok(wallet);
        }

        [HttpPost]
        public async Task<IActionResult> AddWallet(string userId)
        {
            var wallet = await _walletServices.AddWallet(userId);
            if (wallet == "Success")
            {
            return Ok("Add Successfully");
            }
            return BadRequest(wallet);    
        }


        [HttpPut]
        public async Task<IActionResult> UpdateWallet(string? walletId, [FromBody]WalletRequestDTO walletRequestDTO)
        {
            if (walletId == null)
            {
                return BadRequest("You need to Enter Id");
            }
            var wallet = await _walletServices.UpdateWallets(walletId, walletRequestDTO);
            if(wallet == "Success")
            {
                return Ok("Update Successfully");
            }
            return BadRequest(wallet);
        }

    }
}
