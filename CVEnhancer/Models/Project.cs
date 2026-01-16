namespace CVEnhancer.Models
{
    public class Project
    {
        public int ProjectId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ProjectUrl { get; set; }
        public List<Skill> Skills { get; set; } = new();
    }
}
