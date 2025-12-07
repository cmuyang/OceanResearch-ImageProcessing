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
        // 接收批量提交： [{ ImageId, Choice }, ... ]
        [HttpPost("bulk")]
        [Authorize]
        public async Task<IActionResult> PostBulk([FromBody] List<SelectionDto> items)
        {
            // 从JWT令牌中获取用户ID声明（NameIdentifier通常包含用户ID）
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Forbid();

            // 将用户ID字符串转换为整数
            var uid = int.Parse(userIdClaim);

            foreach (var dto in items)
            {
                // 若已存在该用户对该图片的选择 -> 更新；否则插入新纪录
                var existing = await _db.Selections.FirstOrDefaultAsync(s => s.UserId == uid && s.ImageId == dto.ImageId);
                if (existing != null)
                {
                    existing.Choice = dto.Choice;
                    existing.CreatedAt = DateTime.UtcNow;
                    _db.Selections.Update(existing);
                }
                else
                {
                    var sel = new Selection
                    {
                        ImageId = dto.ImageId,
                        UserId = uid,
                        Choice = dto.Choice,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _db.Selections.AddAsync(sel);
                }
            }

            await _db.SaveChangesAsync();
            return Ok();
        }

        public class SelectionDto
        {
            public int ImageId { get; set; }
            public string Choice { get; set; } = "";
        }
    }
}
