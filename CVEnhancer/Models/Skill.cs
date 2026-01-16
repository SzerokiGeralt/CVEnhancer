namespace CVEnhancer.Models
{
    public class Skill
    {
        public int SkillId { get; set; }
        public string Name { get; set; }
        public SkillCategory Category { get; set; }
        public List<SkillAlias> Aliases { get; set; } = new ();
        public List<WorkExperience> WorkExperiences { get; set; } = new ();
        public List<Project> Projects { get; set; } = new ();
        public List<Certificate> Certificates { get; set; } = new ();
        public List<Education> Educations { get; set; } = new ();
    }
}
