using AppManageTasks.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AppManageTasks.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserTask> UserTasks { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=addressCoordinates;Username=your_username;Password=your_password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTask>().ToTable("user_tasks");
        }
    }
}