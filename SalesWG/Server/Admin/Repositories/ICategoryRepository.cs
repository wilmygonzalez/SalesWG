using SalesWG.Server.Data;
using SalesWG.Server.Repositories;
using SalesWG.Shared.Admin.Models.Catalog.Category;
using SalesWG.Shared.Models;

namespace SalesWG.Server.Admin.Repositories
{
    public interface ICategoryRepository :IRepository<Category>
    {
        Task<AppResponse<PagedResponse<CategoryResponse>>> GetAllCategories(int pageIndex, int pageSize, string searchString);
        Task<AppResponse<IEnumerable<ParentCategory>>> GetParentCategoriesBySearch(string searchString, int categoryId = 0);
        Task<AppResponse<CategoryResponse>> GetCategoryById(int id);
        Task<AppResponse<CategoryResponse>> InsertCategory(AddEditCategoryRequest request);
        Task<AppResponse<CategoryResponse>> UpdateCategory(AddEditCategoryRequest request);
        Task<AppResponse<CategoryResponse>> DeleteCategory(int id);
    }
}
