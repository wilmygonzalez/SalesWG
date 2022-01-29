using System.ComponentModel.DataAnnotations;

namespace SalesWG.Shared.Models.Admin.Catalog.Category
{
    public class AddEditCategoryRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public ParentCategory? ParentCategory { get; set; }
    }
}
