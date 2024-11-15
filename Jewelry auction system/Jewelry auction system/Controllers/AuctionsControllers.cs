using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/Auctions")]
    [ApiController]
    public class AuctionsControllers : ControllerBase
    {
        private readonly IAuctionServices _auctionServices;

        public AuctionsControllers(IAuctionServices auctionServices)
        {
            _auctionServices = auctionServices;
        }

        #region Get all auction with img
        [HttpGet("GetAllAuctions-Pending")]
        public async Task<ActionResult<List<AuctionDetailsDTO>>> GetAuctions_PendingStatusAndSearchTerm([FromQuery] string? searchTerm)
        {
            try
            {
                var auctionDetails = await _auctionServices.GetAuctionsByStatusAndSearchTermAsync(AuctionStatus.Pending, searchTerm);
                return Ok(auctionDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllAuctions-Active")]
        public async Task<ActionResult<List<AuctionDetailsDTO>>> GetAuctions_ActiveStatusAndSearchTerm([FromQuery] string? searchTerm)
        {
            try
            {
                var auctionDetails = await _auctionServices.GetAuctionsByStatusAndSearchTermAsync(AuctionStatus.Active, searchTerm);
                return Ok(auctionDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllAuctions-Done")]
        public async Task<ActionResult<List<AuctionDetailsDTO>>> GetAuctions_DoneStatusAndSearchTerm([FromQuery] string? searchTerm)
        {
            try
            {
                var auctionDetails = await _auctionServices.GetAuctionsByStatusAndSearchTermAsync(AuctionStatus.Done, searchTerm);
                return Ok(auctionDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetAllAuctions-None")]
        public async Task<ActionResult<List<AuctionDetailsDTO>>> GetAuctions_NoneStatusAndSearchTerm([FromQuery] string? searchTerm)
        {
            try
            {
                var auctionDetails = await _auctionServices.GetAuctionsByStatusAndSearchTermAsync(AuctionStatus.None, searchTerm);
                return Ok(auctionDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region Get auction detail with img
        [HttpGet("GetAuctionById")]
        public async Task<ActionResult<AuctionDetailsDTO>> GetAuctionById(string id)
        {
            try
            {
                var auctionDetails = await _auctionServices.GetAuctionDetailsAsync(id);
                return Ok(auctionDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion


        [Authorize(Policy = "Staff")]
        [HttpPost("AddAuctions")]
        public async Task<IActionResult> AddAuctions(string? productId, AuctionsDTO auctionsDTO)
        {
            if(productId == null)
            {
                return BadRequest("You need to enter the ProductID");
            }
                var result = await _auctionServices.AddAuction(productId,auctionsDTO);
                if(result == "Success")
            {
                return Ok("Create Auction Successful");
            }
            else
            {
                return BadRequest(result);
            }                    
        }

        [Authorize(Policy = "Manager")]
        [HttpPut]
        public async Task<IActionResult> UpdateAuctions(string auctionId, AuctionUpdateDTO auctionRequestDTO)
        {
            var result = await _auctionServices.UpdateAuction(auctionId, auctionRequestDTO);
            if (result == "Success")
            {
                return Ok("Update Successfully");
            }
            return BadRequest(result);
        }


        [HttpPut("Status")]
        public async Task<IActionResult> UpdateAuctionStatus(string auctionId, [FromBody] AuctionStatus newStatus)
        {
            bool result = await _auctionServices.UpdateStatusAuction(auctionId, newStatus);
            if (result)
            {
                return Ok("Product status updated successfully.");
            }
            else
            {
                return NotFound("Product not found or invalid status transition.");
            }
        }
             
    }
}
