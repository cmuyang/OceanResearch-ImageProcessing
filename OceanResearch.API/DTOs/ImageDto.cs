namespace OceanResearch.API.DTOs
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string Metadata { get; set; }
        public bool IsClearest { get; set; }      // 是否标记为"最清晰"
        public bool HasResearchValue { get; set; } // 是否标记为"有研究价值"
        public bool ShouldRemove { get; set; }    // 是否标记为"剔除"
    }

    //public class ImageSummaryDto
    //{
    //    public string Metadata { get; set; } = string.Empty; // 图片唯一标识 (文件夹/文件名)
    //    public string Url { get; set; } = string.Empty;      // 图片访问路径
        
    //    // 统计数据
    //    public int TotalAnnotations { get; set; }
    //    public int ClearestCount { get; set; }
    //    public int ResearchValueCount { get; set; }
    //    public int RemoveCount { get; set; }
        
    //    public DateTime LastUpdated { get; set; } // 最后有人标注的时间
    //}
}
