namespace CVEnhancer.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? ProfessionalSummary { get; set; }
        public string? JobTitle { get; set; }
        public List<Project> Projects { get; set; } = new ();
        public List<Certificate> Certificates { get; set; } = new ();
        public List<WorkExperience> WorkExperiences { get; set; } = new ();
        public List<Education> Educations { get; set; } = new ();
    }
}
