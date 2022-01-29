using Microsoft.EntityFrameworkCore;

namespace SalesWG.Server.Data
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Category> Category { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
