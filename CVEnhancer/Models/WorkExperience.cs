namespace CVEnhancer.Models
{
    public class WorkExperience
    {
        public int WorkExperienceId { get; set; }
        public User User { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public List<Skill> Skills { get; set; } = new();
    }
}
