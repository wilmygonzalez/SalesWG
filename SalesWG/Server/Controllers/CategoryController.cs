using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesWG.Server.Interfaces.Repositories;
using SalesWG.Shared.Data;
using SalesWG.Shared.Models;

namespace SalesWG.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryController(
            IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: api/Categories
        [HttpGet("GetCategories")]
        public async Task<IEnumerable<Category>> GetCategories()
        {
            var categories = await _categoryRepository
                .GetAll()
                .ToListAsync();

            return categories;
        }

        [HttpGet("GetCategoriesByName/{name}")]
        public async Task<IEnumerable<ParentCategory>> GetCategoriesByName(string name)
        {
            var parentCategories = await _categoryRepository
                .FindAsync(x => x.Name.ToLower().Contains(name.ToLower()))
                .Select(x => new ParentCategory
                {
                    Id = x.Id,
                    Name = x.Name
                }).Take(10).ToListAsync();

            return parentCategories;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(long id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category == null)
                return NotFound();

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(long id, Category category)
        {
            if (id != category.Id)
                return BadRequest();

            var categoryExists = await CategoryExists(id);

            if (!categoryExists)
                return NotFound();

            await _categoryRepository.UpdateAsync(category);

            return Ok();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            await _categoryRepository.InsertAsync(category);

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(long id)
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
