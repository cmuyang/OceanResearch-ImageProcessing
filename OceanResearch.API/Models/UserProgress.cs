using System;

namespace OceanResearch.API.Models
{
    public class UserProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CurrentPage { get; set; } = 1; // 当前标注到的页码
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

