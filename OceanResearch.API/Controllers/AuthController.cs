using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OceanResearch.API.DTOs;
using OceanResearch.API.Data;
using OceanResearch.API.DTOs;
using OceanResearch.API.Services;
using OceanResearch.API.Models;

namespace OceanResearch.API.Controllers
{
    [ApiController]//API控制器
    [Route("api/[controller]")]//定义路由模板，[controller]会被替换为“Auth”
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJwtService _jwt;
        // 构造函数，通过依赖注入获取服务实例
        public AuthController(AppDbContext db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }
        // 处理用户登录的POST请求，路由为 api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req) // 从请求体获取登录数据
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == req.Username);
            // 如果用户不存在，返回401未授权状态
            if (user == null) return Unauthorized(new { message = "Invalid credentials" });
            // 使用BCrypt验证密码是否匹配，不匹配，返回401未授权状态
            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials" });
            // 生成JWT令牌
            var token = _jwt.GenerateToken(user);
            // 返回登录成功响应，包含令牌、用户名和用户ID
            return Ok(new LoginResponse { Token = token, Username = user.Username, UserId = user.Id });
        }
        //处理登出

        // Optional register for testing
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginRequest req)// 从请求体获取注册数据
        {
            // 检查用户名是否已存在
            if (await _db.Users.AnyAsync(u => u.Username == req.Username))
                return BadRequest(new { message = "用户已存在" });
            if (req == null ||
                string.IsNullOrWhiteSpace(req.Username) ||
                string.IsNullOrWhiteSpace(req.Password))
            {
                return BadRequest(new { message = "用户名和密码不能为空" });
            }

            // Basic password strength check (adjust as needed)
            if (req.Password.Length < 6)
                return BadRequest(new { message = "密码长度至少 6 位" });

            // Normalize username (you may want to trim / lower-case depending on policy)
            var username = req.Username.Trim();

            // 创建新用户对象
            var user = new Models.User
            {
                Username = req.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password)// 使用BCrypt哈希密码
            };
            // 将新用户添加到数据库上下文
            _db.Users.Add(user);
            // 保存更改到数据库
            await _db.SaveChangesAsync();
            //自动登录
            var token = _jwt.GenerateToken(user);
            return CreatedAtAction(nameof(Register), new { id = user.Id }, new LoginResponse
            {
                Token = token,
                Username = user.Username,
                UserId = user.Id
            });
            // 返回200成功状态
            return Ok();
        }
    }
}
