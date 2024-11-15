using Azure;
using Jewelry_auction_system.Data.Entity;
using JewelryAuction.Data.Enums;
using JewelryAuction.Data.Models;
using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Data.Models.ResponseDTO;
using JewelryAuction.Services.Services;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/Product")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductServices _services;

        public ProductsController(IProductServices services)
        {
            _services = services;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProduct(string? searchterm, string? filter)
        {
            try
            {
                var product = await _services.GetAllProduct(searchterm, filter);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductbyId(string id)
        {
            try
            {
                var product = await _services.GetProductsById(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("id/listType")]
        public async Task<IActionResult> GetProductbyIdAsync(string id)
        {
            var product = await _services.GetProductbyIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductRequestDTO productDTO, string? ProductID)
        {
            if (ProductID == null)
            {
                return BadRequest("You need enter ProuctID");
            }
            try
            {
                var result = await _services.UpdateProduct(ProductID, productDTO);
                return Ok(result);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("change-status")]
        public async Task<IActionResult> UpdaterStatus(string productID, [FromBody] ProductStatus newStatus)
        {

            bool result = await _services.UpdaterStatusProduct(productID, newStatus);
            if (result)
            {
                return Ok("Product status updated successfully.");
            }
            else
            {
                return NotFound("Product not found or invalid status transition.");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProduct(string? ProductID)
        {
            if(ProductID == null)
            {
                return BadRequest("You need enter ProductID");
            }
            try
            {
                var result = await _services.DeleteProduct(ProductID);
                return Ok(result);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Get all products by userId
        [HttpGet("ProductByUserId/{userId}")]
        public async Task<IActionResult> GetAllProductsByUserId(string userId)
        {
            try
            {
                var products = await _services.GetAllProductsByUserIdAsync(userId);
                if (products == null || !products.Any())
                {
                    return NotFound(new { message = "No products found for this user." });
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        #endregion

        #region Get product by status

        //Status = Requesting
        [HttpGet("ProductByStatus/Requesting")]
        public async Task<ActionResult<List<ProductsByStatusDTO>>> GetAllProductsByStatus_Requesting()
        {
            try
            {
                var products = await _services.GetAllProductsByStatus_Requesting();
                return Ok(products);
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

        //Status = Accepted
        [HttpGet("ProductByStatus/Accepted")]
        public async Task<ActionResult<List<ProductsByStatusDTO>>> GetAllProductsByStatus_Accepted()
        {
            try
            {
                var products = await _services.GetAllProductsByStatus_Accepted();
                return Ok(products);
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

        //Status = Denied
        [HttpGet("ProductByStatus/Denied")]
        public async Task<ActionResult<List<ProductsByStatusDTO>>> GetAllProductsByStatus_Denied()
        {
            try
            {
                var products = await _services.GetAllProductsByStatus_Denied();
                return Ok(products);
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


        //Status = Hold
        [HttpGet("ProductByStatus/Hold")]
        public async Task<ActionResult<List<ProductsByStatusDTO>>> GetAllProductsByStatus_Hold()
        {
            try
            {
                var products = await _services.GetAllProductsByStatus_Hold();
                return Ok(products);
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

        //Status = Sold
        [HttpGet("ProductByStatus/Sold")]
        public async Task<ActionResult<List<ProductsByStatusDTO>>> GetAllProductsByStatus_Sold()
        {
            try
            {
                var products = await _services.GetAllProductsByStatus_Sold();
                return Ok(products);
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
    }
}
