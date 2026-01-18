using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVEnhancer.DTO
{
    public class MatchingItemDTO
    {
        public string Type { get; set; } = ""; // "Project" / "WorkExperience"
        public int SourceId { get; set; }      // ProjectId / WorkExperienceId
        public string Title { get; set; } = ""; // nazwa projektu / stanowisko+firma
        public double Score { get; set; }      // cosine similarity 0..1
        public List<string> MatchedKeywords { get; set; } = new();
    }

    public class MatchingResultDTO
    {
        public double OverallScore { get; set; } // np. średnia z top N
        public List<string> JobKeywords { get; set; } = new();
        public List<MatchingItemDTO> TopProjects { get; set; } = new();
        public List<MatchingItemDTO> TopWorkExperiences { get; set; } = new();
    }
}
