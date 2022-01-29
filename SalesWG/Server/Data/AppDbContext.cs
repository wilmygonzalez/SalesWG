using Microsoft.EntityFrameworkCore;
using SalesWG.Shared.Data;
using SalesWG.Shared.Models;

namespace SalesWG.Server.Data
{
    public class AppDbContext : DbContext
    {
        public virtual DbSet<Category> Category { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
