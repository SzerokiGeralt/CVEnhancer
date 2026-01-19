using CVEnhancer.Interfaces;

namespace CVEnhancer.Models
{
    public class Education : IMatchable
    {
        public int EducationId { get; set; }
        public User User { get; set; }
        public string InstitutionName { get; set; }
        public string Degree { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FieldOfStudy { get; set; }
        public List<Skill> Skills { get; set; } = new();

        // IMatchable implementation
        public int Id => EducationId;
        public string MatchableType => "Education";
        public string DisplayTitle => $"{Degree} - {FieldOfStudy}";
        public string DisplayDescription => InstitutionName;

        public string GetSearchableText()
        {
            var parts = new List<string> { InstitutionName, Degree, FieldOfStudy };
            parts.AddRange(Skills.Select(s => s.Name));
            parts.AddRange(Skills.SelectMany(s => s.Aliases ?? new List<string>()));
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public IEnumerable<string> GetSkillNames() => Skills.Select(s => s.Name);
    }
}
