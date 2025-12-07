using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OceanResearch.API.DTOs;
using OceanTeseach.API.Data;
using OceanTeseach.API.DTOs;
using OceanTeseach.API.Models;
using System.Security.Claims;

namespace OceanTeseach.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SelectionsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SelectionsController(AppDbContext db) { _db = db; }

        [HttpPost]
        public async Task<IActionResult> SubmitSelection([FromBody] SelectionDto dto)
        {
            // 从JWT令牌中获取用户ID声明（NameIdentifier通常包含用户ID）
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            // 将用户ID字符串转换为整数
            var uid = int.Parse(userIdClaim);
            // 创建新的选择记录对象
            var selection = new Selection
            {
                UserId = uid,
                ImageId = dto.ImageId,
                Choice = dto.Choice
            };
            //将新选择增加到数据库
            _db.Selections.Add(selection);
            //保存更改到数据库
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
