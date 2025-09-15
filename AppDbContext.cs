using Microsoft.EntityFrameworkCore;

namespace OfficeSuit.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProjectModel> Projects { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }
    }
}