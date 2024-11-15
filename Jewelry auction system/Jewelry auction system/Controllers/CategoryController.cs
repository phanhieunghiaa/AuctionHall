using Azure;
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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _services;

        public CategoryController(ICategoryServices services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var categories = await _services.GetAllCategories();
            if (categories == null)
            {
                return NotFound();
            }
            return Ok(categories);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetAllCategoryById(string? id)
        {
            if(id == null)
            {
                return BadRequest("You need Enter id");
            }
            var catagories = await _services.GetCategoriesById(id);
            if (catagories == null)
            {
                return NotFound();
            }
            return Ok(catagories);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategories(CategoriesDTO categoriesDTO)
        {
            if(categoriesDTO == null)
            {
                return BadRequest("Categories Not Found");
            }
            var categories = await _services.AddCategories(categoriesDTO);
            if(categories.Success)
            {
                return Ok(categories.SuccessMessage ?? "Category added successfully."); ;
            }
            return BadRequest(categories.ErrorMessage);
        }

        [HttpPost("ListType")]
        public async Task<IActionResult> AddCategoryTypes([FromBody] CategoryTypeRequestDTO categoryTypeAddition)
        {
            try
            {
                await _services.AddListCategoryType(categoryTypeAddition);
                return Ok("Category types added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteCategories(string id)
        {
            if (id == null)
            {
                return BadRequest("You need enter ProductID");
            }
            try
            {
                var result = await _services.DeleteCategories(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
