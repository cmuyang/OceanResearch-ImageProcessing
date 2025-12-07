using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OceanResearch.API.Data;
using OceanResearch.API.DTOs;
using System.Security.Claims;

namespace OceanResearch.API.Controllers
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
        [Authorize]
        public async Task<IActionResult> GetImages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Forbid();
            var uid = int.Parse(userIdClaim);

            var items = await _db.Images
                .OrderBy(i => i.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var imageIds = items.Select(i => i.Id).ToList();
            var selections = await _db.Selections
                .Where(s => s.UserId == uid && imageIds.Contains(s.ImageId))
                .ToListAsync();

            var dtos = items.Select(i =>
            {
                var sel = selections.FirstOrDefault(s => s.ImageId == i.Id);
                return new ImageDto
                {
                    Id = i.Id,
                    FileName = i.FileName,
                    Url = i.Url,
                    Metadata = i.Metadata,
                    SelectedChoice = sel?.Choice
                };
            }).ToList();

            return Ok(dtos);
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
