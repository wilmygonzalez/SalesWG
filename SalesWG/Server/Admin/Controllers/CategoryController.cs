using Microsoft.AspNetCore.Mvc;
using SalesWG.Server.Admin.Repositories;
using SalesWG.Server.Helpers;
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

        [HttpGet]
        [Route("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories([FromQuery] PagedRequest request)
        {
            var result = await _categoryRepository.GetAllCategories(request.PageIndex, request.PageSize, request.SearchString);

            return Ok(result);
        }

        [HttpGet("GetParentCategoriesBySearch/{stringSearch}/{categoryId}")]
        public async Task<IActionResult> GetParentCategoriesBySearch(string stringSearch, int categoryId)
        {
            var result = await _categoryRepository.GetParentCategoriesBySearch(stringSearch, categoryId);
            return Ok(result);
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryRepository.GetCategoryById(id);
            return Ok(result);
        }

        [HttpPost("InsertCategory")]
        public async Task<IActionResult> InsertCategory(AddEditCategoryRequest request)
        {
            var result = await _categoryRepository.InsertCategory(request);
            return Ok(result);
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(AddEditCategoryRequest category)
        {
            var result = await _categoryRepository.UpdateCategory(category);

            return Ok(result);
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryRepository.DeleteCategory(id);

            return Ok(result);
        }
    }
}
