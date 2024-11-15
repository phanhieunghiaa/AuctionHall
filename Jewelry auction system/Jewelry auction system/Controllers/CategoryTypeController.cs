using JewelryAuction.Data.Models;
using JewelryAuction.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jewelry_auction_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryTypeController : ControllerBase
    {
        private readonly ICategoryTypeServices _categoryTypeServices;

        public CategoryTypeController(ICategoryTypeServices categoryTypeServices)
        {
            _categoryTypeServices = categoryTypeServices;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllType()
        {
            var type = await _categoryTypeServices.GetAllType();
            if (type == null)
            {
                return NotFound();
            }
            return Ok(type);    
        }

        [HttpPost]
        public async Task<IActionResult> AddType(CategoryTypeDTO categoryTypeDTO)
        {
            if (categoryTypeDTO == null)
            {
                return BadRequest("Categories Not Found");
            }
            var type = await _categoryTypeServices.AddType(categoryTypeDTO);
            if (type.Success)
            {
                return Ok(type.SuccessMessage ?? "Type added successfully."); ;
            }
            return BadRequest(type.ErrorMessage);
        }
    }
}
