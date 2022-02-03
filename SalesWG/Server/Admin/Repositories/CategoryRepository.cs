using Microsoft.EntityFrameworkCore;
using SalesWG.Server.Data;
using SalesWG.Server.Helpers;
using SalesWG.Server.Repositories;
using SalesWG.Shared.Admin.Helpers;
using SalesWG.Shared.Admin.Models.Catalog.Category;
using SalesWG.Shared.Admin.Requests.Catalog.Category;
using SalesWG.Shared.Admin.Responses.Catalog.Category;
using SalesWG.Shared.Helpers;
using SalesWG.Shared.Requests;

namespace SalesWG.Server.Admin.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext) { }

        public async Task<IPagedList<CategoryResponse>> GetAllCategories(int pageIndex, int pageSize, string searchString)
        {
            var categories = await base.GetAll()
                .Include(x => x.Parent)
                .Where(x => string.IsNullOrEmpty(searchString) || x.Name.ToLower().Contains(searchString) ||
                        x.Description.ToLower().Contains(searchString))
                .Select(x => new CategoryResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ParentCategory = x.Parent != null ? new ParentCategory
                    {
                        Id = x.Parent.Id,
                        Name = x.Parent.Name
                    } : null
                })
                .ToPagedListAsync(pageIndex, pageSize);

            return categories;
        }

        public async Task<CategoryResponse> GetCategoryById(int id)
        {
            var category = await base.GetByIdAsync(id);

            var categoryResponse = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ParentCategory = category.Parent != null ? new ParentCategory
                {
                    Id = category.Parent.Id,
                    Name = category.Parent.Name
                } : null
            };

            return categoryResponse;
        }

        public async Task<IEnumerable<ParentCategory>> GetParentCategoriesBySearch(string searchString)
        {
            var parentCategories = await base.FindAsync(x => x.Name.ToLower().Contains(searchString.ToLower()))
                .Select(x => new ParentCategory
                {
                    Id = x.Id,
                    Name = x.Name
                }).Take(10).ToListAsync();

            return parentCategories;
        }

        public async Task InsertCategory(AddEditCategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentCategory?.Id
            };

            await base.InsertAsync(category);
        }

        public async Task UpdateCategory(AddEditCategoryRequest request)
        {
            var category = await base.GetByIdAsync(request.Id);

            category.Name = request.Name;
            category.Description = request.Description;
            category.ParentId = request.ParentCategory?.Id;

            await base.InsertAsync(category);
        }
    }
}
