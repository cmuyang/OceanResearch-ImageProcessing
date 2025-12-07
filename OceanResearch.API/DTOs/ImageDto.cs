namespace OceanResearch.API.DTOs
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public string Metadata { get; set; }
        public string? SelectedChoice { get; set; }
    }
}
