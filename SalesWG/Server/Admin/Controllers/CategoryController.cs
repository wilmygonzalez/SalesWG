using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesWG.Server.Admin.Repositories;
using SalesWG.Server.Data;
using SalesWG.Server.Helpers;
using SalesWG.Server.Repositories;
using SalesWG.Shared.Helpers;
using SalesWG.Shared.Admin.Models.Catalog.Category;
using SalesWG.Shared.Admin.Requests.Catalog.Category;
using SalesWG.Shared.Admin.Responses.Catalog.Category;
using SalesWG.Shared.Requests;

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
        public async Task<PagedResponse<CategoryResponse>> GetAllCategories([FromQuery] PagedRequest request)
        {
            var categories = await _categoryRepository.GetAllCategories(request.PageIndex, request.PageSize, request.SearchString);

            return categories.ToPagedResponse();
        }

        [HttpGet("GetParentCategoriesBySearch/{stringSearch}")]
        public async Task<IEnumerable<ParentCategory>> GetParentCategoriesBySearch(string stringSearch)
        {
            return await _categoryRepository.GetParentCategoriesBySearch(stringSearch);
        }

        [HttpGet("GetCategoryById/{id}")]
        public async Task<CategoryResponse> GetCategoryById(int id)
        {
            if (id == 0)
                return null;

            return await _categoryRepository.GetCategoryById(id);
        }

        [HttpPost("InsertCategory")]
        public async Task<IActionResult> InsertCategory(AddEditCategoryRequest request)
        {
            await _categoryRepository.InsertCategory(request);

            return Ok();
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(AddEditCategoryRequest category)
        {
            var categoryExists = await CategoryExists(category.Id);

            if (!categoryExists)
                return NotFound();

            await _categoryRepository.UpdateCategory(category);

            return Ok();
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            await _categoryRepository.DeleteAsync(category);

            return Ok();
        }

        private async Task<bool> CategoryExists(long id)
        {
            return await _categoryRepository.Exists(id);
        }
    }
}
