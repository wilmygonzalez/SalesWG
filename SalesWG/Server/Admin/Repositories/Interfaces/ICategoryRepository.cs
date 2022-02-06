using SalesWG.Server.Data;
using SalesWG.Server.Repositories;
using SalesWG.Shared.Admin.Models.Catalog.Category;
using SalesWG.Shared.Models;

namespace SalesWG.Server.Admin.Repositories
{
    public interface ICategoryRepository :IAppRepository<Category>
    {
        Task<AppResponse<PagedResponse<CategoryResponse>>> GetAllCategoriesAsync(int pageIndex, int pageSize, string searchString);
        Task<AppResponse<IEnumerable<ParentCategory>>> GetParentCategoriesBySearchAsync(string searchString, int categoryId = 0);
        Task<AppResponse<CategoryResponse>> GetCategoryByIdAsync(int id);
        Task<AppResponse<CategoryResponse>> InsertCategoryAsync(AddEditCategoryRequest request);
        Task<AppResponse<CategoryResponse>> UpdateCategoryAsync(AddEditCategoryRequest request);
        Task<AppResponse<CategoryResponse>> DeleteCategoryAsync(int id);
    }
}
