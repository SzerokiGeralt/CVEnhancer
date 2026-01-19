using CVEnhancer.Interfaces;

namespace CVEnhancer.Models
{
    public class Project : IMatchable
    {
        public int ProjectId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? ProjectUrl { get; set; }
        public List<Skill> Skills { get; set; } = new();

        // IMatchable implementation
        public int Id => ProjectId;
        public string MatchableType => "Project";
        public string DisplayTitle => Name;
        public string DisplayDescription => Description ?? "";

        public string GetSearchableText()
        {
            var parts = new List<string> { Name, Description ?? "" };
            parts.AddRange(Skills.Select(s => s.Name));
            parts.AddRange(Skills.SelectMany(s => s.Aliases ?? new List<string>()));
            if (!string.IsNullOrEmpty(ProjectUrl))
                parts.Add(ProjectUrl);
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public IEnumerable<string> GetSkillNames() => Skills.Select(s => s.Name);
    }
}
