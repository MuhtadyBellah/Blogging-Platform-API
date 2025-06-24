using System.Reflection;
using Default_Project.Cores.Models;
using Microsoft.EntityFrameworkCore;
namespace Default_Project.Repos.Data
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Has> Has { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
