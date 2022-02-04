using Microsoft.EntityFrameworkCore;
using SalesWG.Server.Data;
using SalesWG.Server.Helpers;
using SalesWG.Server.Repositories;
using SalesWG.Shared.Admin.Models.Catalog.Category;
using SalesWG.Shared.Models;

namespace SalesWG.Server.Admin.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext appDbContext) : base(appDbContext) { }

        public async Task<AppResponse<PagedResponse<CategoryResponse>>> GetAllCategories(int pageIndex, int pageSize, string searchString)
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

            var response = new AppResponse<PagedResponse<CategoryResponse>> 
            { 
                Data = categories.ToPagedResponse() 
            };

            return response;
        }

        public async Task<AppResponse<IEnumerable<ParentCategory>>> GetParentCategoriesBySearch(string searchString)
        {
            var parentCategories = await base.FindAsync(x => x.Name.ToLower().Contains(searchString.ToLower()))
                .Select(x => new ParentCategory
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .Take(10)
                .ToListAsync();

            return new AppResponse<IEnumerable<ParentCategory>> { Data = parentCategories };
        }

        public async Task<AppResponse<CategoryResponse>> GetCategoryById(int id)
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

            return new AppResponse<CategoryResponse> { Data = categoryResponse };
        }

        public async Task<AppResponse<CategoryResponse>> InsertCategory(AddEditCategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentCategory?.Id
            };

            await base.InsertAsync(category);

            return new AppResponse<CategoryResponse> { Message = "Category created." };
        }

        public async Task<AppResponse<CategoryResponse>> UpdateCategory(AddEditCategoryRequest request)
        {
            var category = await base.GetByIdAsync(request.Id);
            if (category == null)
            {
                return new AppResponse<CategoryResponse>
                {
                    Message = "This category doesn't exist.",
                    Success = false
                };
            }

            category.Name = request.Name;
            category.Description = request.Description;
            category.ParentId = request.ParentCategory?.Id;

            await base.InsertAsync(category);

            return new AppResponse<CategoryResponse> { Message = "Category updated." };
        }

        public async Task<AppResponse<CategoryResponse>> DeleteCategory(int id)
        {
            var category = await base.GetByIdAsync(id);

            if (category == null)
            {
                return new AppResponse<CategoryResponse>
                {
                    Message = "This category cannot be deleted.",
                    Success = false
                };
            }

            await base.DeleteAsync(category);

            return new AppResponse<CategoryResponse> { Message = "Category deleted." };
        }

        private async Task<bool> CategoryExists(long id)
        {
            return await base.Exists(id);
        }
    }
}
