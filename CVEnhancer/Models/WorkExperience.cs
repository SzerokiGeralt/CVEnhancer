using CVEnhancer.Interfaces;

namespace CVEnhancer.Models
{
    public class WorkExperience : IMatchable
    {
        public int WorkExperienceId { get; set; }
        public User User { get; set; }
        public string CompanyName { get; set; }
        public string JobTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public List<Skill> Skills { get; set; } = new();

        // IMatchable implementation
        public int Id => WorkExperienceId;
        public string MatchableType => "WorkExperience";
        public string DisplayTitle => $"{JobTitle} @ {CompanyName}";
        public string DisplayDescription => Description ?? "";

        public string GetSearchableText()
        {
            var parts = new List<string> { CompanyName, JobTitle, Description ?? "" };
            parts.AddRange(Skills.Select(s => s.Name));
            parts.AddRange(Skills.SelectMany(s => s.Aliases ?? new List<string>()));
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public IEnumerable<string> GetSkillNames() => Skills.Select(s => s.Name);
    }
}
