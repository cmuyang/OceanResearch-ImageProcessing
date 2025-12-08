using Microsoft.EntityFrameworkCore;
using OceanResearch.API.Models;

namespace OceanResearch.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        // 定义Users表对应的DbSet，用于对用户数据进行CRUD操作
        public DbSet<User> Users { get; set; }
        public DbSet<ImageRecord> Images { get; set; }
        public DbSet<Selection> Selections { get; set; }
        public DbSet<UserProgress> UserProgresses { get; set; }
        // 重写OnModelCreating方法，用于配置数据模型和数据库映射
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 调用基类方法，确保基类的配置也被应用
            base.OnModelCreating(modelBuilder);
            // 确保数据库中的用户名不会重复
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        }
    }
}
