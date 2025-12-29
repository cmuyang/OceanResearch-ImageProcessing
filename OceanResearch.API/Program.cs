//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.FileProviders;
//using Microsoft.IdentityModel.Tokens;
//using OceanResearch.API.Data;
//using OceanResearch.API.Services;
//using System.Reflection;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Jwt
//var secret = builder.Configuration["Jwt:Secret"] ?? throw new Exception("Jwt:Secret not configured");
//var key = Encoding.ASCII.GetBytes(secret);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.RequireHttpsMetadata = false;
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = false,
//        ValidateAudience = false,
//    };
//});

//builder.Services.AddScoped<IJwtService, JwtService>();

//var app = builder.Build();

//// Ensure wwwroot exists
//if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
//    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

//// Serve static files for images
//app.UseStaticFiles();

//// Serve static files for uieb-instances folder
//var uiebInstancesPath = Path.Combine(builder.Environment.ContentRootPath, "..", "uieb-instances");
//if (Directory.Exists(uiebInstancesPath))
//{
//    app.UseStaticFiles(new StaticFileOptions
//    {
//        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uiebInstancesPath),
//        RequestPath = "/uieb-instances"
//    });
//}

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();

//// Seed data
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
//    db.Database.Migrate();
//    DataSeeder.Seed(db, Path.Combine(env.WebRootPath ?? "wwwroot", "images"));
//}

//app.Run();
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using OceanResearch.API.Data;
using OceanResearch.API.Services;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// 保持原样，不添加额外配置
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Jwt
var secret = builder.Configuration["Jwt:Secret"] ?? throw new Exception("Jwt:Secret not configured");
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// Ensure wwwroot exists
if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));

// Serve static files for images
app.UseStaticFiles();

// Serve static files for uieb-instances folder
var uiebInstancesPath = Path.Combine(builder.Environment.ContentRootPath, "..", "uieb-instances");
if (Directory.Exists(uiebInstancesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uiebInstancesPath),
        RequestPath = "/uieb-instances"
    });
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    // 修改这里：配置 SwaggerUI 使用根路径
    app.UseSwaggerUI(options =>
    {
        // 指定 Swagger JSON 文件的位置（默认是 v1）
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        // 将路由前缀设置为空，这样访问 http://localhost:xxxx/ 就会直接显示 Swagger 页面
        options.RoutePrefix = string.Empty;
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    db.Database.Migrate();
    DataSeeder.Seed(db, Path.Combine(env.WebRootPath ?? "wwwroot", "images"));
}

app.Run();