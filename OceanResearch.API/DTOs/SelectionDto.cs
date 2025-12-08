namespace OceanResearch.API.DTOs
{
    public class SelectionDto
    {
        public int ImageId { get; set; }
        public string? Metadata { get; set; }
        public bool IsClearest { get; set; }      // 是否标记为"最清晰"
        public bool HasResearchValue { get; set; } // 是否标记为"有研究价值"
        public bool ShouldRemove { get; set; }    // 是否标记为"剔除"
    }
}
