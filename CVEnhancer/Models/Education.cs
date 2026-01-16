namespace CVEnhancer.Models
{
    public class Education
    {
        public int EducationId { get; set; }
        public User User { get; set; }
        public string InstitutionName { get; set; }
        public string Degree { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FieldOfStudy { get; set; }
        public List<Skill> Skills { get; set; } = new();
    }
}
