namespace CVEnhancer.Models
{
    public class GeneratedCV
    {
        public int GeneratedCVId { get; set; }
        public User User { get; set; }
        public string Title { get; set; }
        public string JsonFilePath { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
