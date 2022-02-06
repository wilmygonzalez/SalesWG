using Microsoft.AspNetCore.Mvc;
using SalesWG.Server.Admin.Repositories;
using SalesWG.Shared.Admin.Models.Catalog.Category;
using SalesWG.Shared.Models;

namespace SalesWG.Server.Admin.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(
            ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] PagedRequest request)
        {
            var response = await _categoryRepository.GetAllCategoriesAsync(request.PageIndex, request.PageSize, request.SearchString);

            return Ok(response);
        }

        [HttpGet("GetParentCategoriesBySearch/{stringSearch}/{categoryId}")]
        public async Task<IActionResult> GetParentCategoriesBySearch(string stringSearch, int categoryId)
        {
            var response = await _categoryRepository.GetParentCategoriesBySearchAsync(stringSearch, categoryId);
            return Ok(response);
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var response = await _categoryRepository.GetCategoryByIdAsync(id);
            return Ok(response);
        }

        [HttpPost("InsertCategory")]
        public async Task<IActionResult> InsertCategory(AddEditCategoryRequest request)
        {
            var response = await _categoryRepository.InsertCategoryAsync(request);
            return Ok(response);
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(AddEditCategoryRequest category)
        {
            var response = await _categoryRepository.UpdateCategoryAsync(category);

            return Ok(response);
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var response = await _categoryRepository.DeleteCategoryAsync(id);

            return Ok(response);
        }
    }
}
