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
        public DbSet<ProjectResource> ProjectResources { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Tasks> Tasks { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationship
            modelBuilder.Entity<ProjectModel>()
                .HasOne(p => p.Manager)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.ProjectManagerID);

        }
    }
}