using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVEnhancer.Interfaces;

namespace CVEnhancer.DTO
{
    /// <summary>
    /// Reprezentuje pojedynczy dopasowany element z wynikiem podobieństwa.
    /// </summary>
    public class MatchedItemDTO
    {
        /// <summary>
        /// Oryginalny obiekt implementujący IMatchable.
        /// </summary>
        public IMatchable Item { get; set; } = null!;

        /// <summary>
        /// Wynik podobieństwa kosinusowego (0..1).
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Lista słów kluczowych, które pasują między ofertą a elementem.
        /// </summary>
        public List<string> MatchedKeywords { get; set; } = new();

        /// <summary>
        /// Skille z elementu, które pasują do oferty.
        /// </summary>
        public List<string> MatchedSkills { get; set; } = new();

        // Convenience properties dla UI
        public string Type => Item?.MatchableType ?? "";
        public int SourceId => Item?.Id ?? 0;
        public string Title => Item?.DisplayTitle ?? "";
        public string Description => Item?.DisplayDescription ?? "";
    }

    /// <summary>
    /// Wynik analizy dopasowania CV do oferty pracy.
    /// </summary>
    public class MatchingResultDTO
    {
        /// <summary>
        /// Ogólny wynik dopasowania (średnia z top N).
        /// </summary>
        public double OverallScore { get; set; }

        /// <summary>
        /// Słowa kluczowe wyodrębnione z oferty pracy (top TF-IDF terms).
        /// </summary>
        public List<string> JobKeywords { get; set; } = new();

        /// <summary>
        /// Skille wykryte w ofercie pracy (kanoniczne nazwy po preprocessingu).
        /// </summary>
        public List<string> DetectedJobSkills { get; set; } = new();

        /// <summary>
        /// Top N najlepiej dopasowanych elementów (wszystkie typy razem).
        /// </summary>
        public List<MatchedItemDTO> TopMatches { get; set; } = new();

        /// <summary>
        /// Dopasowane elementy pogrupowane według typu.
        /// </summary>
        public Dictionary<string, List<MatchedItemDTO>> MatchesByType { get; set; } = new();

        // Convenience properties dla łatwego dostępu do konkretnych typów
        public List<MatchedItemDTO> TopProjects => 
            MatchesByType.TryGetValue("Project", out var list) ? list : new();
        
        public List<MatchedItemDTO> TopWorkExperiences => 
            MatchesByType.TryGetValue("WorkExperience", out var list) ? list : new();
        
        public List<MatchedItemDTO> TopCertificates => 
            MatchesByType.TryGetValue("Certificate", out var list) ? list : new();
        
        public List<MatchedItemDTO> TopEducation => 
            MatchesByType.TryGetValue("Education", out var list) ? list : new();
    }
}
