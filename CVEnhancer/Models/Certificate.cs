using CVEnhancer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CVEnhancer.Models
{
    public class Certificate : IMatchable
    {
        public int CertificateId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string IssuingOrganization { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string CredentialId { get; set; }
        public string? CredentialUrl { get; set; }
        public List<Skill> Skills { get; set; } = new();

        // IMatchable implementation
        public int Id => CertificateId;
        public string MatchableType => "Certificate";
        public string DisplayTitle => Name;
        public string DisplayDescription => $"Issued by {IssuingOrganization}";

        public string GetSearchableText()
        {
            var parts = new List<string> { Name, IssuingOrganization };
            parts.AddRange(Skills.Select(s => s.Name));
            parts.AddRange(Skills.SelectMany(s => s.Aliases ?? new List<string>()));
            return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        public IEnumerable<string> GetSkillNames() => Skills.Select(s => s.Name);
    }
}
