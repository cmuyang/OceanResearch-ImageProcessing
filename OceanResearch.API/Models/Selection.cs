//using System;

//namespace OceanResearch.API.Models
//{
//    public class Selection
//    {
//        public int Id { get; set; }

//        public int UserId { get; set; }
//        public User User { get; set; }

//        public int ImageId { get; set; }
//        public ImageRecord Image { get; set; }

//        public string Choice { get; set; }

//        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
//    }
//}
using System;

namespace OceanResearch.API.Models
{
    public class Selection
    {
        public int Id { get; set; }
        public int ImageId { get; set; }          // 对应 ImageRecord.Id
        public int UserId { get; set; }        // 登录用户标识（可用 username 或 user id）
        public string? Metadata { get; set; }     // 存储图片标识，如 "文件夹名/文件名"
        public bool IsClearest { get; set; }      // 是否标记为"最清晰"
        public bool HasResearchValue { get; set; } // 是否标记为"有研究价值"
        public bool ShouldRemove { get; set; }    // 是否标记为"剔除"  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
