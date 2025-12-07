using System;
using System.IO;
using System.Linq;
using OceanResearch.API.Models;

namespace OceanResearch.API.Data
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext db, string wwwrootImagesFolder)
        {
            db.Database.EnsureCreated();
            // 检查Users表是否为空，如果为空则添加初始用户数据
            if (!db.Users.Any())
            {
                // 使用BCrypt加密默认密码
                var pwHash = BCrypt.Net.BCrypt.HashPassword("password123");
                // 创建测试用户并添加到数据库
                db.Users.Add(new User { Username = "testuser", PasswordHash = pwHash });
                db.SaveChanges();
            }
            // 检查Images表是否为空，如果为空则添加初始图像数据
            if (!db.Images.Any())
            {
                if (!Directory.Exists(wwwrootImagesFolder))
                {
                    Directory.CreateDirectory(wwwrootImagesFolder);
                    // Note: add a README reminding to put images here.
                }
                // 获取图像文件夹中的所有图像文件
                var files = Directory.GetFiles(wwwrootImagesFolder)
                    .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    .Take(50);

                foreach (var f in files)
                {
                    var fileName = Path.GetFileName(f);
                    var img = new ImageRecord
                    {
                        FileName = fileName,
                        Url = $"/images/{fileName}",
                        Metadata = $"seeded from {fileName}"
                    };
                    db.Images.Add(img);
                }
                db.SaveChanges();
            }
        }
    }
}
