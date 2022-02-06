using System.ComponentModel.DataAnnotations;

namespace SalesWG.Server.Data
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; }
    }
}
