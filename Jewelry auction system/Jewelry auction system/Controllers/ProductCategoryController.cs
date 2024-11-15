using JewelryAuction.Data.Models.ReqpuestDTO;
using JewelryAuction.Services.Services;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryServices _services;

        public  ProductCategoryController(IProductCategoryServices services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductHaveCategory(string? searchterm, string categoryTypeId)
        {
            if (categoryTypeId == null)
            {
                return BadRequest("You needs enter ID.");
            }
            var product = await _services.GetAllProductInCateGory(searchterm, categoryTypeId);
            if (product == null)
            {
                return NotFound("Product is not existed");
            }
            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> AddProductHaveCategory(string produtcId, string categoryTypeId)
        {
            if (produtcId == null)
            {
                return BadRequest("You needs enter ProductID.");
            }
            if (categoryTypeId == null)
            {
                return BadRequest("You needs enter CategoryID");
            }
            var product = await _services.AddProductInCateGories(produtcId, categoryTypeId);
            if (product == "Success")
            {
                return NotFound("Add Product has category Successfully!");
            }
            return BadRequest(product);
        }

        [HttpPost("ListType")]
        public async Task<IActionResult> AddProductWithCategoryTypes([FromBody] ProductWithCategoryTypesDTO productDTO)
        {
            if (productDTO == null || productDTO.CategoryTypeIds == null || !productDTO.CategoryTypeIds.Any())
            {
                return BadRequest("Invalid product data or category types.");
            }
            var response = await _services.AddProductWithCategoryTypesAsync(productDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response.ErrorMessage);
            }
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetProductHasCategory(string? productId, string? categoryTypeId)
        {
            if (productId == null)
            {
                return BadRequest("You need enter ProductID");
            }
            if (categoryTypeId == null)
            {
                return BadRequest("You need enter CategoryID");
            }
            var productinCategory = await _services.GetProductCategoryAsync(productId, categoryTypeId);
            if (productinCategory == null)
            {
                return NotFound("Product not has category");
            }
            return Ok(productinCategory);
        }

        [HttpPut("productId")]
        public async Task<IActionResult> UpdateProductWithCategoryTypes(string productId, [FromBody] ProductWithCategoryTypesDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _services.UpdateProductWithCategoryType(productDTO, productId);

            if (!response.Success)
            {
                return BadRequest(response.ErrorMessage);
            }

            return Ok(response.Data);
        }

    }
}
