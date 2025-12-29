using Microsoft.EntityFrameworkCore;
using OceanResearch.API.Models;

namespace OceanResearch.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ImageRecord> Images { get; set; }
        public DbSet<Selection> Selections { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }

        // 新增聚合表 DbSet
        public DbSet<ImageAnnotationSummary> ImageAnnotationSummaries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();

            // 为 Metadata 建唯一索引，方便快速查找/更新
            modelBuilder.Entity<ImageAnnotationSummary>()
                .HasIndex(s => s.Metadata)
                .IsUnique();
        }
    }
}
