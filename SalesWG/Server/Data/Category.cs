using System.ComponentModel.DataAnnotations;

namespace SalesWG.Server.Data
{
    public class Category : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public int? ParentId { get; set; }
        public virtual Category? Parent { get; set; }
        public virtual ICollection<Category> Childrens { get; set;}
    }
}
