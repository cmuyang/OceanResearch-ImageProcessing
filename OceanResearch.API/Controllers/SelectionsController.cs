using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OceanResearch.API.Data;
using OceanResearch.API.DTOs;
using OceanResearch.API.Models;
using System.Security.Claims;

namespace OceanResearch.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SelectionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SelectionsController(AppDbContext db) { _db = db; }

        //[HttpPost]
        //public async Task<IActionResult> SubmitSelection([FromBody] SelectionDto dto)
        //{
        //    // 从JWT令牌中获取用户ID声明（NameIdentifier通常包含用户ID）
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
        //    // 将用户ID字符串转换为整数
        //    var uid = int.Parse(userIdClaim);
        //    // 创建新的选择记录对象
        //    var selection = new Selection
        //    {
        //        UserId = uid,
        //        ImageId = dto.ImageId,
        //        Choice = dto.Choice
        //    };
        //    //将新选择增加到数据库
        //    _db.Selections.Add(selection);
        //    //保存更改到数据库
        //    await _db.SaveChangesAsync();
        //    return Ok();
        //}
        // 接收批量提交： [{ ImageId, Metadata, IsClearest, HasResearchValue, ShouldRemove }, ... ]
        [HttpPost("bulk")]
        [Authorize]
        public async Task<IActionResult> PostBulk([FromBody] List<SelectionDto> items, [FromQuery] int? currentPage = null)
        {
            // 从JWT令牌中获取用户ID声明（NameIdentifier通常包含用户ID）
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Forbid();

            // 将用户ID字符串转换为整数
            var uid = int.Parse(userIdClaim);

            // 获取当前页的文件名（用于确保同一页只能有一张图片标记为"最清晰"）
            string? currentPageFileName = null;
            if (currentPage.HasValue && items.Any())
            {
                // 从第一个item的Metadata中提取文件名
                var firstMetadata = items.FirstOrDefault()?.Metadata;
                if (!string.IsNullOrEmpty(firstMetadata))
                {
                    var parts = firstMetadata.Split('/');
                    if (parts.Length >= 2)
                    {
                        currentPageFileName = parts[1]; // 文件名部分
                    }
                }
            }

            // 如果当前页有标记为"最清晰"的图片，先清除该页所有图片的"最清晰"标记
            if (!string.IsNullOrEmpty(currentPageFileName))
            {
                // 获取该页所有图片的Metadata
                var pageMetadata = items.Where(i => !string.IsNullOrEmpty(i.Metadata) && i.Metadata.EndsWith($"/{currentPageFileName}"))
                    .Select(i => i.Metadata!)
                    .ToList();
                
                if (pageMetadata.Any())
                {
                    // 清除该页所有图片的"最清晰"标记
                    var existingClearest = await _db.Selections
                        .Where(s => s.UserId == uid && s.IsClearest && pageMetadata.Contains(s.Metadata!))
                        .ToListAsync();
                    
                    foreach (var existing in existingClearest)
                    {
                        existing.IsClearest = false;
                        _db.Selections.Update(existing);
                    }
                }
            }

            foreach (var dto in items)
            {
                Selection? existing = null;
                
                // 优先使用Metadata查找，如果没有Metadata则使用ImageId
                if (!string.IsNullOrEmpty(dto.Metadata))
                {
                    existing = await _db.Selections.FirstOrDefaultAsync(s => s.UserId == uid && s.Metadata == dto.Metadata);
                }
                else if (dto.ImageId > 0)
                {
                    existing = await _db.Selections.FirstOrDefaultAsync(s => s.UserId == uid && s.ImageId == dto.ImageId);
                }

                if (existing != null)
                {
                    existing.IsClearest = dto.IsClearest;
                    existing.HasResearchValue = dto.HasResearchValue;
                    existing.ShouldRemove = dto.ShouldRemove;
                    existing.CreatedAt = DateTime.UtcNow;
                    _db.Selections.Update(existing);
                }
                else
                {
                    var sel = new Selection
                    {
                        ImageId = dto.ImageId,
                        UserId = uid,
                        Metadata = dto.Metadata,
                        IsClearest = dto.IsClearest,
                        HasResearchValue = dto.HasResearchValue,
                        ShouldRemove = dto.ShouldRemove,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _db.Selections.AddAsync(sel);
                }
            }

            // 保存进度
            if (currentPage.HasValue)
            {
                var progress = await _db.UserProgresses.FirstOrDefaultAsync(p => p.UserId == uid);
                if (progress == null)
                {
                    progress = new UserProgress
                    {
                        UserId = uid,
                        CurrentPage = currentPage.Value,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _db.UserProgresses.AddAsync(progress);
                }
                else
                {
                    progress.CurrentPage = currentPage.Value;
                    progress.UpdatedAt = DateTime.UtcNow;
                    _db.UserProgresses.Update(progress);
                }
            }

            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
