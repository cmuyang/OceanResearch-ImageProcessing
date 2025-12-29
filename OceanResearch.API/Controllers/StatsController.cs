using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OceanResearch.API.Data;
using OceanResearch.API.DTOs;

namespace OceanResearch.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")] // 这里定义了路由前缀为 api/stats
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StatsController(AppDbContext db)
        {
            _db = db;
        }

        // 获取所有图片的标注统计情况
        // GET: api/stats
        [HttpGet]
        public async Task<IActionResult> GetAnnotationStats(
            [FromQuery] string sortBy = "recent",
            [FromQuery] string filter = "all",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _db.ImageAnnotationSummaries.AsQueryable();

            // 1. 筛选逻辑
            switch (filter.ToLower())
            {
                case "exclude_removed":
                    query = query.Where(s => s.RemoveCount == 0);
                    break;
                case "only_clearest":
                    query = query.Where(s => s.ClearestCount > 0);
                    break;
                case "only_research":
                    query = query.Where(s => s.ResearchValueCount > 0);
                    break;
                case "research_not_clearest":
                    query = query.Where(s => s.ResearchValueCount > 0 && s.ClearestCount == 0);
                    break;
                case "all":
                default:
                    break;
            }

            // 2. 排序逻辑
            query = sortBy.ToLower() switch
            {
                "clearest" => query.OrderByDescending(s => s.ClearestCount).ThenByDescending(s => s.LastUpdated),
                "research" => query.OrderByDescending(s => s.ResearchValueCount).ThenByDescending(s => s.LastUpdated),
                "remove" => query.OrderByDescending(s => s.RemoveCount).ThenByDescending(s => s.LastUpdated),
                _ => query.OrderByDescending(s => s.LastUpdated)
            };

            // 3. 分页与返回
            var total = await query.CountAsync();
            var summaries = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = summaries.Select(s => new ImageSummaryDto
            {
                Metadata = s.Metadata,
                Url = $"/uieb-instances/{s.Metadata}",
                TotalAnnotations = s.TotalAnnotations,
                ClearestCount = s.ClearestCount,
                ResearchValueCount = s.ResearchValueCount,
                RemoveCount = s.RemoveCount,
                LastUpdated = s.LastUpdated
            }).ToList();

            return Ok(new
            {
                TotalCount = total,
                Filter = filter,
                SortBy = sortBy,
                Page = page,
                PageSize = pageSize,
                Items = dtos
            });
        }
    }
}