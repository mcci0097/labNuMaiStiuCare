using Microsoft.EntityFrameworkCore;

namespace lab2_restapi_1205_taskmgmt.Models
{
    // dbcontext = unit of work, collection of repositories. Even if we work with multiple operation it will
    // completed as a single operation
    public class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity => {
                entity.HasIndex(u => u.Username).IsUnique();
            });
        }

        // DbSet = Repository
        // DbSet = O tabela din baza de date
        public DbSet<Task> Tasks { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<User> Users { get; set; }
    }
}