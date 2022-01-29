using System.ComponentModel.DataAnnotations;

namespace SalesWG.Shared.Data
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public int ParentCategoryId { get; set; }
    }
}
