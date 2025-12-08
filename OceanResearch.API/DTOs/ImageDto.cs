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
}
