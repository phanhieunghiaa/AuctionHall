using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Models;
using JewelryAuction.Services.Services;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/FeedBacks")]
    [ApiController]
    public class FeedBacksController : ControllerBase
    {
        private readonly IFeedBackServices _feedBackServices;

        public FeedBacksController(IFeedBackServices feedBackServices)
        {
            _feedBackServices = feedBackServices;
        }

        [HttpPost("add-feedback")]
        public async Task<IActionResult> AddFeedBack(string userId, string auctionId, [FromBody] FeedBackContentDTO feedbackContent)
        {
            try
            {
                await _feedBackServices.AddFeedback(userId, auctionId, feedbackContent);
                return Ok("Add feedback sucessfully.");
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

        [HttpPost("edit-feedback")]
        public async Task<IActionResult> EditFeedBack(string userId, string auctionId, [FromBody] FeedBackContentDTO feedbackContent)
        {
            try
            {
                await _feedBackServices.EditFeedBack(userId, auctionId, feedbackContent);
                return Ok("Edit feedback sucessfully.");
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

        [HttpPost("disable-feedback")]
        public async Task<IActionResult> DisableFeedBack(string userId, string auctionId)
        {
            try
            {
                await _feedBackServices.DisableFeedBack(userId, auctionId);
                return Ok("Disable feedback sucessfully.");
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

        [HttpGet("view-feedback(id)")]
        public async Task<IActionResult> ViewFeedBackById(string fbId)
        {
            try
            {
                var feedback = await _feedBackServices.GetFeedbackById(fbId);
                return Ok(feedback);
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

        [HttpGet("viewAll-feedback(auctionId)")]
        public async Task<IActionResult> GetAllFeedbacksByAuctionId(string auctionId)
        {
            try
            {
                var feedbackDTOs = await _feedBackServices.GetAllFeedbacksByAuctionId(auctionId);
                return Ok(feedbackDTOs);
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
    }
}
