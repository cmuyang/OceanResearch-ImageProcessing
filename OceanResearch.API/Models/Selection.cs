using System;

namespace OceanTeseach.API.Models
{
    public class Selection
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ImageId { get; set; }
        public ImageRecord Image { get; set; }

        public string Choice { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
