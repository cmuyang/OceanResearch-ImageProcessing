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
        public string Choice { get; set; }        // 例如 "最清晰" / "剔除"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
