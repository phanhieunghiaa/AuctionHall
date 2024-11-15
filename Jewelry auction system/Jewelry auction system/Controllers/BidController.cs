using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Services.Services;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/Bids")]
    [ApiController]
    public class BidController : ControllerBase
    {
        private readonly IBidServices _bidServices;

        public BidController(IBidServices bidServices)
        {
            _bidServices = bidServices;
        }

        #region Place bid
        [HttpPost("place-bid")]
        public async Task<IActionResult> PlaceBid(string auctionId, string userId, float bidValue)
        {
            try
            {
                await _bidServices.PlaceBid(auctionId, userId, bidValue);
                return Ok("Bid placed successfully.");
            }
            catch (KeyNotFoundException knfex)
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region Cancel Bid
        [HttpPost("cancel-bid")]
        public async Task<IActionResult> CancelBid(string bidId)
        {
            try
            {
                await _bidServices.CancelBid(bidId);
                return Ok("Bid cancelled successfully.");
            }
            catch (KeyNotFoundException knfex)
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region Get all bid by user Id
        [HttpGet("BidByUserId")]
        public async Task<IActionResult> GetBidsByUserId(string userId)
        {
            try
            {
                var bidDTOs = await _bidServices.GetBidsByUserIdAsync(userId);
                return Ok(bidDTOs);
            }
            catch (KeyNotFoundException knfex)
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region View auction all bid

        [HttpGet("{auctionId}")]
        public async Task<IActionResult> GetBidsByAuctionId(string auctionId)
        {
            try
            {
                var bids = await _bidServices.GetBidsByAuctionIdAsync(auctionId);
                return Ok(bids);
            }
            catch (KeyNotFoundException knfex)
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region View auction highest bid
        [HttpGet("HighestBid{auctionId}")]
        public async Task<IActionResult> ViewAuctionHighestBid(string auctionId)
        {
            try
            {
                var bid = await _bidServices.ViewHighestBidByAuctionId(auctionId);
                return Ok(bid);
            }
            catch (KeyNotFoundException knfex)
            {
                return NotFound(knfex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion
    }
}
