using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OceanTeseach.API.Data;
using OceanTeseach.API.DTOs;

namespace OceanTeseach.API.Controllers
{
    [ApiController]
    [Authorize]//所有请求必须通过身份验证，需要有效的jwt令牌
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly AppDbContext _db;
        // 构造函数，通过依赖注入获取数据库上下文
        public ImagesController(AppDbContext db) { _db = db; }
        // 处理获取图像列表的GET请求，路由为 api/images
        [HttpGet]
        public async Task<IActionResult> GetImages(int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;
            // 从数据库查询图像数据
            var items = await _db.Images
                .OrderBy(i => i.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new ImageDto { Id = i.Id, FileName = i.FileName, Url = i.Url, Metadata = i.Metadata })
                .ToListAsync();

            return Ok(items);
        }
        // 处理获取特定图像Base64数据的GET请求，路由为 api/images/{id}/base64--------------------------------
        [HttpGet("{id}/base64")]
        public async Task<IActionResult> GetImageBase64(int id)
        {
            var img = await _db.Images.FindAsync(id);
            if (img == null) return NotFound();

            //if (!string.IsNullOrEmpty(img.Base64Data))
            //    return Ok(new { id = img.Id, base64 = img.Base64Data });
            // 移除URL开头的斜杠，获取相对路径
            var relative = img.Url?.TrimStart('/');
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relative ?? "");
            if (!System.IO.File.Exists(path)) return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(path);
            var b64 = Convert.ToBase64String(bytes);
            return Ok(new { id = img.Id, base64 = b64 });
        }
    }
}
