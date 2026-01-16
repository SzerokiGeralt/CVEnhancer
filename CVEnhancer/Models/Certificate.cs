using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace CVEnhancer.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public string IssuingOrganization { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string CredentialId { get; set; }
        public string? CredentialUrl { get; set; }
        public List<Skill> Skills { get; set; } = new ();
    }
}
