using CVEnhancer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVEnhancer.Models;

namespace CVEnhancer.Services
{
    public class MatchingService
    {
        private readonly ExtractionService _extraction;

        public MatchingService(ExtractionService extractionService)
        {
            _extraction = extractionService;
        }

        public MatchingResultDTO Match(User userWithData, string jobText, int topN = 3)
        {
            // 1) Budujemy dokumenty: 0 = job, potem projekty, potem work
            var docs = new List<(string DocId, string Type, int SourceId, string Title, string Text)>();

            docs.Add(("job", "Job", 0, "JobOffer", jobText));

            foreach (var p in userWithData.Projects)
            {
                var text = $"{p.Name} {p.Description} {string.Join(" ", p.Skills.Select(s => s.Name))} {p.ProjectUrl}";
                docs.Add(($"project:{p.ProjectId}", "Project", p.ProjectId, p.Name, text));
            }

            foreach (var w in userWithData.WorkExperiences)
            {
                var title = $"{w.JobTitle} @ {w.CompanyName}";
                var text = $"{w.CompanyName} {w.JobTitle} {w.Description} {string.Join(" ", w.Skills.Select(s => s.Name))}";
                docs.Add(($"work:{w.WorkExperienceId}", "WorkExperience", w.WorkExperienceId, title, text));
            }

            // 2) Tokenizacja
            var tokenized = docs.Select(d => _extraction.Tokenize(d.Text)).ToList();

            // 3) TF-IDF dla całego korpusu
            var vectorizer = new TfidfVectorizer();
            var tfidfVectors = vectorizer.FitTransform(tokenized);

            // 4) Wektor ogłoszenia to indeks 0
            var jobVec = tfidfVectors[0];

            // 5) Porównujemy job z każdym innym dokumentem
            var scored = new List<MatchingItemDTO>();

            for (int i = 1; i < docs.Count; i++)
            {
                var score = CosineSimilarity(jobVec, tfidfVectors[i]);

                // matched keywords: przecięcie top słów job i top słów dokumentu
                var jobTop = vectorizer.GetTopTerms(jobVec, 15);
                var docTop = vectorizer.GetTopTerms(tfidfVectors[i], 15);
                var matched = jobTop.Intersect(docTop).Take(10).ToList();

                scored.Add(new MatchingItemDTO
                {
                    Type = docs[i].Type,
                    SourceId = docs[i].SourceId,
                    Title = docs[i].Title,
                    Score = score,
                    MatchedKeywords = matched
                });
            }

            var topProjects = scored
                .Where(x => x.Type == "Project")
                .OrderByDescending(x => x.Score)
                .Take(topN)
                .ToList();

            var topWork = scored
                .Where(x => x.Type == "WorkExperience")
                .OrderByDescending(x => x.Score)
                .Take(topN)
                .ToList();

            // OverallScore: średnia z top wyników (żeby było KPI-friendly)
            var topAll = scored.OrderByDescending(x => x.Score).Take(Math.Max(1, topN)).ToList();
            var overall = topAll.Count == 0 ? 0 : topAll.Average(x => x.Score);

            // keywords z ogłoszenia (dla UI)
            var jobKeywords = vectorizer.GetTopTerms(jobVec, 25);

            return new MatchingResultDTO
            {
                OverallScore = overall,
                JobKeywords = jobKeywords,
                TopProjects = topProjects,
                TopWorkExperiences = topWork
            };
        }

        // --- Cosine similarity ---
        private static double CosineSimilarity(Dictionary<int, double> a, Dictionary<int, double> b)
        {
            if (a.Count == 0 || b.Count == 0) return 0;

            // dot product po wspólnych kluczach
            double dot = 0;
            foreach (var kv in a)
            {
                if (b.TryGetValue(kv.Key, out var bv))
                    dot += kv.Value * bv;
            }

            double normA = Math.Sqrt(a.Values.Sum(x => x * x));
            double normB = Math.Sqrt(b.Values.Sum(x => x * x));

            if (normA == 0 || normB == 0) return 0;
            return dot / (normA * normB);
        }

        // --- Minimalny TF-IDF wewnątrz (bez paczek) ---
        private class TfidfVectorizer
        {
            private Dictionary<string, int> _vocab = new(StringComparer.Ordinal);
            private double[] _idf = Array.Empty<double>();

            public List<Dictionary<int, double>> FitTransform(List<List<string>> corpusTokens)
            {
                BuildVocab(corpusTokens);
                ComputeIdf(corpusTokens);

                var vectors = new List<Dictionary<int, double>>(corpusTokens.Count);

                for (int docIndex = 0; docIndex < corpusTokens.Count; docIndex++)
                {
                    var tf = new Dictionary<int, double>();
                    var tokens = corpusTokens[docIndex];

                    if (tokens.Count == 0)
                    {
                        vectors.Add(new Dictionary<int, double>());
                        continue;
                    }

                    // term frequency
                    var counts = tokens
                        .GroupBy(t => t)
                        .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

                    foreach (var (term, count) in counts)
                    {
                        if (!_vocab.TryGetValue(term, out int id)) continue;

                        // TF: count / totalTokens
                        var tfValue = (double)count / tokens.Count;

                        // TF-IDF
                        var value = tfValue * _idf[id];
                        if (value != 0)
                            tf[id] = value;
                    }

                    vectors.Add(tf);
                }

                return vectors;
            }

            public List<string> GetTopTerms(Dictionary<int, double> vector, int k)
            {
                return vector
                    .OrderByDescending(kv => kv.Value)
                    .Take(k)
                    .Select(kv => _vocab.First(x => x.Value == kv.Key).Key) // mało wydajne, ale korpus mały
                    .ToList();
            }

            private void BuildVocab(List<List<string>> corpusTokens)
            {
                _vocab.Clear();
                int idx = 0;
                foreach (var doc in corpusTokens)
                {
                    foreach (var token in doc)
                    {
                        if (!_vocab.ContainsKey(token))
                            _vocab[token] = idx++;
                    }
                }
            }

            private void ComputeIdf(List<List<string>> corpusTokens)
            {
                int nDocs = corpusTokens.Count;
                _idf = new double[_vocab.Count];

                // document frequency
                var df = new int[_vocab.Count];

                foreach (var doc in corpusTokens)
                {
                    foreach (var term in doc.Distinct())
                    {
                        if (_vocab.TryGetValue(term, out int id))
                            df[id]++;
                    }
                }

                // IDF: log((N + 1) / (df + 1)) + 1  (smoothing)
                for (int i = 0; i < df.Length; i++)
                {
                    _idf[i] = Math.Log((nDocs + 1.0) / (df[i] + 1.0)) + 1.0;
                }
            }
        }
    }
}
