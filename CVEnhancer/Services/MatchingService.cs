using CVEnhancer.DTO;
using CVEnhancer.Models;
using CVEnhancer.Interfaces;

namespace CVEnhancer.Services
{
    public class MatchingService
    {
        private readonly ExtractionService _extraction;
        private readonly JobTextPreprocessor _preprocessor;

        public MatchingService(ExtractionService extractionService, JobTextPreprocessor preprocessor)
        {
            _extraction = extractionService;
            _preprocessor = preprocessor;
        }

        /// <summary>
        /// Asynchronicznie dopasowuje elementy użytkownika do oferty pracy.
        /// </summary>
        /// <param name="userWithData">Użytkownik z załadowanymi danymi</param>
        /// <param name="jobText">Tekst oferty pracy</param>
        /// <param name="topN">Liczba najlepszych dopasowań do zwrócenia</param>
        /// <returns>Wynik dopasowania</returns>
        public async Task<MatchingResultDTO> MatchAsync(User userWithData, string jobText, int topN = 10)
        {
            // 1) Preprocessing tekstu oferty pracy (normalizacja + zamiana aliasów)
            var preprocessedJob = await _preprocessor.PreprocessAsync(jobText);

            // 2) Wykonaj matching w tle
            return await Task.Run(() => MatchInternal(userWithData, preprocessedJob, topN));
        }

        private MatchingResultDTO MatchInternal(User userWithData, PreprocessedJobText preprocessedJob, int topN)
        {
            // 1) Zbierz wszystkie matchable elementy użytkownika
            var matchables = CollectMatchables(userWithData);

            if (matchables.Count == 0)
            {
                return new MatchingResultDTO
                {
                    OverallScore = 0,
                    JobKeywords = preprocessedJob.Tokens.Distinct().Take(25).ToList(),
                    DetectedJobSkills = preprocessedJob.DetectedSkills,
                    TopMatches = new(),
                    MatchesByType = new()
                };
            }

            // 2) Budujemy dokumenty: 0 = job (przetworzona), reszta to matchables
            var docs = new List<(int Index, IMatchable? Item, string Text)>
            {
                (0, null, preprocessedJob.NormalizedText) // Preprocessed job offer at index 0
            };

            foreach (var item in matchables)
            {
                docs.Add((docs.Count, item, item.GetSearchableText()));
            }

            // 3) Tokenizacja
            var tokenized = docs.Select(d => _extraction.Tokenize(d.Text)).ToList();

            // 4) TF-IDF dla całego korpusu
            var vectorizer = new TfidfVectorizer();
            var tfidfVectors = vectorizer.FitTransform(tokenized);

            // 5) Wektor ogłoszenia (indeks 0)
            var jobVec = tfidfVectors[0];
            var jobKeywords = vectorizer.GetTopTerms(jobVec, 25);
            var jobKeywordsSet = new HashSet<string>(jobKeywords, StringComparer.OrdinalIgnoreCase);

            // Dodaj wykryte skille do job keywords set
            foreach (var skill in preprocessedJob.DetectedSkills)
            {
                jobKeywordsSet.Add(skill.ToLowerInvariant());
            }

            // 6) Oblicz similarity dla każdego elementu
            var scored = new List<MatchedItemDTO>();

            for (int i = 1; i < docs.Count; i++)
            {
                var item = docs[i].Item!;
                var docVec = tfidfVectors[i];
                
                var score = CosineSimilarity(jobVec, docVec);

                // Matched keywords: przecięcie top słów job i dokumentu
                var docTop = vectorizer.GetTopTerms(docVec, 15);
                var matchedKeywords = jobKeywords.Intersect(docTop, StringComparer.OrdinalIgnoreCase)
                    .Take(10)
                    .ToList();

                // Matched skills: skille z elementu, które pasują do wykrytych skilli w ofercie
                var itemSkills = item.GetSkillNames().ToList();
                var matchedSkills = itemSkills
                    .Where(skill => 
                        preprocessedJob.DetectedSkills.Any(ds => 
                            ds.Equals(skill, StringComparison.OrdinalIgnoreCase)) ||
                        jobKeywordsSet.Contains(skill) ||
                        jobKeywords.Any(jk => 
                            jk.Contains(skill, StringComparison.OrdinalIgnoreCase) ||
                            skill.Contains(jk, StringComparison.OrdinalIgnoreCase)))
                    .Distinct()
                    .ToList();

                // Bonus do score za dopasowane skille
                var skillBonus = matchedSkills.Count * 0.05; // 5% bonus per matched skill
                var adjustedScore = Math.Min(1.0, score + skillBonus);

                scored.Add(new MatchedItemDTO
                {
                    Item = item,
                    Score = adjustedScore,
                    MatchedKeywords = matchedKeywords,
                    MatchedSkills = matchedSkills
                });
            }

            // 7) Sortuj i weź top N
            var topMatches = scored
                .OrderByDescending(x => x.Score)
                .Take(topN)
                .ToList();

            // 8) Grupuj według typu
            var matchesByType = scored
                .GroupBy(x => x.Type)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.Score).Take(topN).ToList()
                );

            // 9) Overall score: średnia z top matches
            var overallScore = topMatches.Count == 0 ? 0 : topMatches.Average(x => x.Score);

            return new MatchingResultDTO
            {
                OverallScore = overallScore,
                JobKeywords = jobKeywords,
                DetectedJobSkills = preprocessedJob.DetectedSkills,
                TopMatches = topMatches,
                MatchesByType = matchesByType
            };
        }

        /// <summary>
        /// Zbiera wszystkie elementy IMatchable od użytkownika.
        /// </summary>
        private static List<IMatchable> CollectMatchables(User user)
        {
            var result = new List<IMatchable>();

            if (user.Projects != null)
                result.AddRange(user.Projects);

            if (user.WorkExperiences != null)
                result.AddRange(user.WorkExperiences);

            if (user.Certificates != null)
                result.AddRange(user.Certificates);

            if (user.Educations != null)
                result.AddRange(user.Educations);

            return result;
        }

        // --- Cosine similarity ---
        private static double CosineSimilarity(Dictionary<int, double> a, Dictionary<int, double> b)
        {
            if (a.Count == 0 || b.Count == 0) return 0;

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

        // --- TF-IDF Vectorizer ---
        private class TfidfVectorizer
        {
            private Dictionary<string, int> _vocab = new(StringComparer.Ordinal);
            private double[] _idf = Array.Empty<double>();
            private string[] _reverseVocab = Array.Empty<string>();

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

                    var counts = tokens
                        .GroupBy(t => t)
                        .ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);

                    foreach (var (term, count) in counts)
                    {
                        if (!_vocab.TryGetValue(term, out int id)) continue;

                        var tfValue = (double)count / tokens.Count;
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
                    .Where(kv => kv.Key < _reverseVocab.Length)
                    .Select(kv => _reverseVocab[kv.Key])
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

                // Buduj reverse vocab dla szybkiego lookup
                _reverseVocab = new string[_vocab.Count];
                foreach (var (term, id) in _vocab)
                {
                    _reverseVocab[id] = term;
                }
            }

            private void ComputeIdf(List<List<string>> corpusTokens)
            {
                int nDocs = corpusTokens.Count;
                _idf = new double[_vocab.Count];

                var df = new int[_vocab.Count];

                foreach (var doc in corpusTokens)
                {
                    foreach (var term in doc.Distinct())
                    {
                        if (_vocab.TryGetValue(term, out int id))
                            df[id]++;
                    }
                }

                for (int i = 0; i < df.Length; i++)
                {
                    _idf[i] = Math.Log((nDocs + 1.0) / (df[i] + 1.0)) + 1.0;
                }
            }
        }
    }
}
