using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OceanResearch.API.Data;
using OceanResearch.API.DTOs;
using System.IO;
using System.Security.Claims;

namespace OceanResearch.API.Controllers
{
    [ApiController]
    [Authorize]//所有请求必须通过身份验证，需要有效的jwt令牌
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        // 构造函数，通过依赖注入获取数据库上下文
        public ImagesController(AppDbContext db, IWebHostEnvironment env) 
        { 
            _db = db;
            _env = env;
        }
        // 处理获取图像列表的GET请求，路由为 api/images
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetImages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Forbid();
            var uid = int.Parse(userIdClaim);

            // 从uieb-instances文件夹读取图片
            var uiebInstancesPath = Path.Combine(_env.ContentRootPath, "..", "uieb-instances");
            if (!Directory.Exists(uiebInstancesPath))
            {
                return BadRequest("uieb-instances文件夹不存在");
            }

            // 获取所有子文件夹
            var folders = Directory.GetDirectories(uiebInstancesPath)
                .Select(d => new DirectoryInfo(d).Name)
                .OrderBy(n => n)
                .ToList();

            if (folders.Count == 0)
            {
                return BadRequest("uieb-instances文件夹中没有子文件夹");
            }

            // 从第一个文件夹获取所有图片文件名
            var firstFolderPath = Path.Combine(uiebInstancesPath, folders[0]);
            var allFileNames = Directory.GetFiles(firstFolderPath, "*.png")
                .Select(f => Path.GetFileName(f))
                .OrderBy(f => f)
                .ToList();

            if (allFileNames.Count == 0)
            {
                return BadRequest("没有找到图片文件");
            }

            // 分页：每页显示一组同名图片（10个文件夹中的同名图片）
            var totalGroups = allFileNames.Count;
            var currentPage = Math.Max(1, page);
            var skipGroups = (currentPage - 1);
            
            if (skipGroups >= totalGroups)
            {
                return Ok(new List<ImageDto>());
            }

            // 获取当前页的文件名
            var currentFileName = allFileNames[skipGroups];

            // 从所有文件夹中获取同名图片
            var dtos = new List<ImageDto>();
            var imageIdCounter = 1; // 临时ID，用于标识图片

            foreach (var folder in folders)
            {
                var imagePath = Path.Combine(uiebInstancesPath, folder, currentFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    // 构建URL路径
                    var relativePath = $"uieb-instances/{folder}/{currentFileName}";
                    
                    // 检查数据库中是否有该图片的选择记录
                    // 使用文件名和文件夹名作为唯一标识
                    var imageKey = $"{folder}/{currentFileName}";
                    var selection = await _db.Selections
                        .FirstOrDefaultAsync(s => s.UserId == uid && s.Metadata == imageKey);

                    dtos.Add(new ImageDto
                    {
                        Id = imageIdCounter++,
                        FileName = currentFileName,
                        Url = $"/{relativePath.Replace('\\', '/')}",
                        Metadata = imageKey,
                        IsClearest = selection?.IsClearest ?? false,
                        HasResearchValue = selection?.HasResearchValue ?? false,
                        ShouldRemove = selection?.ShouldRemove ?? false
                    });
                }
            }

            return Ok(dtos);
        }

        // 获取用户当前进度
        [HttpGet("progress")]
        [Authorize]
        public async Task<IActionResult> GetProgress()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Forbid();
            var uid = int.Parse(userIdClaim);

            var progress = await _db.UserProgresses
                .FirstOrDefaultAsync(p => p.UserId == uid);

            if (progress == null)
            {
                // 如果没有进度记录，查找所有用户的最大进度
                var maxProgress = await _db.UserProgresses
                    .OrderByDescending(p => p.CurrentPage)
                    .FirstOrDefaultAsync();
                
                var currentPage = maxProgress?.CurrentPage ?? 1;
                return Ok(new { CurrentPage = currentPage });
            }

            return Ok(new { CurrentPage = progress.CurrentPage });
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
