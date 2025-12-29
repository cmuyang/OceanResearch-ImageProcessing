using System;

namespace OceanResearch.API.Models
{
    public class ImageAnnotationSummary
    {
        public int Id { get; set; }
        // 与 Selection.Metadata 对应，或用 ImageId（int）替代 string 类型
        public string Metadata { get; set; } = null!;
        public int TotalAnnotations { get; set; }        // 有效标注人数（非零标注）
        public int ClearestCount { get; set; }
        public int ResearchValueCount { get; set; }
        public int RemoveCount { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}