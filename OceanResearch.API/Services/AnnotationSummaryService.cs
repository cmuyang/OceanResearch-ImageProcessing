using Microsoft.EntityFrameworkCore;
using OceanResearch.API.Data;
using OceanResearch.API.Models;

namespace OceanResearch.API.Services
{
    public class AnnotationSummaryService
    {
        private readonly AppDbContext _db;
        public AnnotationSummaryService(AppDbContext db) => _db = db;

        // metadatas: 要更新的图片标识集合（Metadata 字符串）
        public async Task UpdateSummariesAsync(IEnumerable<string> metadatas)
        {
            var keys = metadatas?.Where(m => !string.IsNullOrWhiteSpace(m)).Select(m => m.Trim()).Distinct().ToList();
            if (keys == null || keys.Count == 0) return;

            // 从 Selections 中重新聚合计数
            var groups = await _db.Selections
                .Where(s => s.Metadata != null && keys.Contains(s.Metadata))
                .GroupBy(s => s.Metadata)
                .Select(g => new
                {
                    Metadata = g.Key,
                    Total = g.Count(), // 根据业务：是否把“无标注”视为存在记录？这里统计 Selection 行数
                    Clearest = g.Count(x => x.IsClearest),
                    Research = g.Count(x => x.HasResearchValue),
                    Remove = g.Count(x => x.ShouldRemove)
                })
                .ToListAsync();
            if(groups == null)
            {
                // 如果没有任何 Selection 记录，清除对应的 Summary 记录
                var toRemove = await _db.ImageAnnotationSummaries
                    .Where(s => keys.Contains(s.Metadata))
                    .ToListAsync();
                if (toRemove.Count > 0)
                {
                    _db.ImageAnnotationSummaries.RemoveRange(toRemove);
                    await _db.SaveChangesAsync();
                }
                return;
            }
            // 更新/插入聚合表
            foreach (var g in groups)
            {
                
                var summary = await _db.ImageAnnotationSummaries.FirstOrDefaultAsync(x => x.Metadata == g.Metadata);
                if (summary == null)
                {
                    _db.ImageAnnotationSummaries.Add(new ImageAnnotationSummary
                    {
                        Metadata = g.Metadata,
                        TotalAnnotations = g.Total,
                        ClearestCount = g.Clearest,
                        ResearchValueCount = g.Research,
                        RemoveCount = g.Remove,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                else
                {
                    summary.TotalAnnotations = g.Total;
                    summary.ClearestCount = g.Clearest;
                    summary.ResearchValueCount = g.Research;
                    summary.RemoveCount = g.Remove;
                    summary.LastUpdated = DateTime.UtcNow;
                    _db.ImageAnnotationSummaries.Update(summary);
                }
            }

            // 对于 keys 中但没有任何 Selection 的条目，删除或清零（根据你想要的行为）
            var existingSummaries = await _db.ImageAnnotationSummaries.Where(s => keys.Contains(s.Metadata)).ToListAsync();
            var groupedKeys = groups.Select(g => g.Metadata).ToHashSet();
            var toClear = existingSummaries.Where(s => !groupedKeys.Contains(s.Metadata)).ToList();
            foreach (var s in toClear)
            {
                // 选择删除或清零计数，这里删除
                _db.ImageAnnotationSummaries.Remove(s);
            }

            await _db.SaveChangesAsync();
        }
    }
}