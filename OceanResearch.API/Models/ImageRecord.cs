using System;

namespace OceanTeseach.API.Models
{
    public class ImageRecord
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }

        //public string Base64Data { get; set; } // optional
        public string Metadata { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
