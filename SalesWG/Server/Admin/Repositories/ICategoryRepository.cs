using SalesWG.Server.Data;
using SalesWG.Server.Repositories;
using SalesWG.Shared.Admin.Helpers;
using SalesWG.Shared.Admin.Models.Catalog.Category;
using SalesWG.Shared.Admin.Requests.Catalog.Category;
using SalesWG.Shared.Admin.Responses.Catalog.Category;
using SalesWG.Shared.Helpers;
using SalesWG.Shared.Requests;

namespace SalesWG.Server.Admin.Repositories
{
    public interface ICategoryRepository :IRepository<Category>
    {
        Task<IPagedList<CategoryResponse>> GetAllCategories(int pageIndex, int pageSize, string searchString);
        Task<CategoryResponse> GetCategoryById(int id);
        Task<IEnumerable<ParentCategory>> GetParentCategoriesBySearch(string searchString);
        Task InsertCategory(AddEditCategoryRequest request);
        Task UpdateCategory(AddEditCategoryRequest request);
    }
}
