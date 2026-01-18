using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVEnhancer.DTO;
using System.Text.RegularExpressions;

namespace CVEnhancer.Services
{
    public class ExtractionService
    {
        // Minimalna lista stop-words PL/EN (możesz rozbudować)
        private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
        {
            "i","oraz","a","an","the","to","of","for","w","we","z","na","do","się","jest","are",
            "with","without","in","on","at","as","od","za","o","u","po","from","by","will","would",
            "junior","senior","developer","programista","praca","pracy","team","project","projects"
        };

        public ExtractedSkillsDTO ExtractKeywords(string rawText, int maxKeywords = 25)
        {
            var tokens = Tokenize(rawText);

            // proste zliczanie częstości
            var freq = tokens
                .GroupBy(t => t)
                .Select(g => new { Term = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(maxKeywords)
                .Select(x => x.Term)
                .ToList();

            return new ExtractedSkillsDTO { Keywords = freq };
        }

        public List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();

            text = text.ToLowerInvariant();

            // zamień wszystko co nie jest literą/cyfrą na spacje
            text = Regex.Replace(text, @"[^\p{L}\p{Nd}]+", " ");

            var tokens = text
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(t => t.Length >= 2)
                .Where(t => !StopWords.Contains(t))
                .ToList();

            return tokens;
        }
    }
}
