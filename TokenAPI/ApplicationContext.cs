using Microsoft.EntityFrameworkCore;
using TokenAPI.DTO;
using TokenAPI.Models;

namespace TokenAPI
{
    public class ApplicationContext : DbContext
    {        
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}
