using System.ComponentModel.DataAnnotations;
using SalesWG.Shared.Admin.Models.Catalog.Category;

namespace SalesWG.Shared.Admin.Requests.Catalog.Category
{
    public class AddEditCategoryRequest
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public ParentCategory? ParentCategory { get; set; }
    }
}
