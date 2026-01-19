using System.Text.RegularExpressions;

namespace CVEnhancer.Services
{
    /// <summary>
    /// Serwis do preprocessingu tekstu oferty pracy.
    /// Normalizuje tekst i zamienia aliasy skilli na nazwy kanoniczne.
    /// </summary>
    public class JobTextPreprocessor
    {
        private readonly LocalDbService _db;
        private Dictionary<string, string>? _aliasMap;
        private readonly object _lock = new();

        public JobTextPreprocessor(LocalDbService db)
        {
            _db = db;
        }

        /// <summary>
        /// Przetwarza tekst oferty pracy:
        /// 1. Normalizuje tekst (lowercase, usuniêcie znaków specjalnych)
        /// 2. Zamienia aliasy skilli na nazwy kanoniczne
        /// </summary>
        public async Task<PreprocessedJobText> PreprocessAsync(string rawJobText)
        {
            if (string.IsNullOrWhiteSpace(rawJobText))
            {
                return new PreprocessedJobText
                {
                    OriginalText = rawJobText,
                    NormalizedText = "",
                    DetectedSkills = new(),
                    Tokens = new()
                };
            }

            // Za³aduj mapê aliasów (cache)
            await EnsureAliasMapLoadedAsync();

            // 1. Podstawowa normalizacja
            var normalized = NormalizeText(rawJobText);

            // 2. Wykryj i zamieñ aliasy na kanoniczne nazwy
            var (processedText, detectedSkills) = ReplaceAliasesWithCanonicalNames(normalized);

            // 3. Tokenizacja
            var tokens = Tokenize(processedText);

            return new PreprocessedJobText
            {
                OriginalText = rawJobText,
                NormalizedText = processedText,
                DetectedSkills = detectedSkills,
                Tokens = tokens
            };
        }

        /// <summary>
        /// Synchroniczna wersja preprocessingu (u¿ywa wczeœniej za³adowanej mapy).
        /// </summary>
        public PreprocessedJobText Preprocess(string rawJobText, Dictionary<string, string> aliasMap)
        {
            if (string.IsNullOrWhiteSpace(rawJobText))
            {
                return new PreprocessedJobText
                {
                    OriginalText = rawJobText,
                    NormalizedText = "",
                    DetectedSkills = new(),
                    Tokens = new()
                };
            }

            var normalized = NormalizeText(rawJobText);
            var (processedText, detectedSkills) = ReplaceAliasesWithCanonicalNames(normalized, aliasMap);
            var tokens = Tokenize(processedText);

            return new PreprocessedJobText
            {
                OriginalText = rawJobText,
                NormalizedText = processedText,
                DetectedSkills = detectedSkills,
                Tokens = tokens
            };
        }

        private async Task EnsureAliasMapLoadedAsync()
        {
            if (_aliasMap != null) return;

            lock (_lock)
            {
                if (_aliasMap != null) return;
            }

            var map = await _db.GetSkillAliasMapAsync();

            lock (_lock)
            {
                _aliasMap ??= map;
            }
        }

        /// <summary>
        /// Normalizuje tekst: lowercase, zamiana znaków specjalnych na spacje.
        /// </summary>
        private static string NormalizeText(string text)
        {
            text = text.ToLowerInvariant();
            
            // Zachowaj niektóre znaki wa¿ne dla technologii (np. C#, .NET, C++)
            // Zamieñ je na bezpieczne tokeny
            text = text.Replace("c#", " csharp ");
            text = text.Replace("c++", " cplusplus ");
            text = text.Replace(".net", " dotnet ");
            text = text.Replace("node.js", " nodejs ");
            text = text.Replace("vue.js", " vuejs ");
            text = text.Replace("react.js", " reactjs ");
            text = text.Replace("next.js", " nextjs ");
            text = text.Replace("nuxt.js", " nuxtjs ");
            text = text.Replace("express.js", " expressjs ");
            text = text.Replace("nest.js", " nestjs ");
            text = text.Replace("asp.net", " aspnet ");

            // Zamieñ wszystko co nie jest liter¹/cyfr¹ na spacje
            text = Regex.Replace(text, @"[^\p{L}\p{Nd}]+", " ");

            // Usuñ wielokrotne spacje
            text = Regex.Replace(text, @"\s+", " ").Trim();

            return text;
        }

        /// <summary>
        /// Znajduje aliasy w tekœcie i zamienia je na kanoniczne nazwy skilli.
        /// </summary>
        private (string processedText, List<string> detectedSkills) ReplaceAliasesWithCanonicalNames(string text)
        {
            if (_aliasMap == null)
                return (text, new List<string>());

            return ReplaceAliasesWithCanonicalNames(text, _aliasMap);
        }

        private static (string processedText, List<string> detectedSkills) ReplaceAliasesWithCanonicalNames(
            string text, 
            Dictionary<string, string> aliasMap)
        {
            var detectedSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            // Sprawdzaj pojedyncze s³owa
            foreach (var word in words)
            {
                if (aliasMap.TryGetValue(word, out var canonicalName))
                {
                    result.Add(canonicalName.ToLowerInvariant());
                    detectedSkills.Add(canonicalName);
                }
                else
                {
                    result.Add(word);
                }
            }

            // Sprawdzaj te¿ dwu- i trzy-s³owne frazy (np. "machine learning", "entity framework")
            var processedText = string.Join(" ", result);
            
            foreach (var (alias, canonical) in aliasMap)
            {
                if (alias.Contains(' ')) // wielos³owny alias
                {
                    var aliasLower = alias.ToLowerInvariant();
                    if (processedText.Contains(aliasLower, StringComparison.OrdinalIgnoreCase))
                    {
                        processedText = Regex.Replace(
                            processedText, 
                            Regex.Escape(aliasLower), 
                            canonical.ToLowerInvariant(), 
                            RegexOptions.IgnoreCase);
                        detectedSkills.Add(canonical);
                    }
                }
            }

            return (processedText, detectedSkills.ToList());
        }

        /// <summary>
        /// Tokenizuje tekst (podobnie jak ExtractionService, ale bez stop-words).
        /// </summary>
        private static List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) 
                return new List<string>();

            return text
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(t => t.Length >= 2)
                .ToList();
        }

        /// <summary>
        /// Wymusza prze³adowanie mapy aliasów z bazy.
        /// </summary>
        public void InvalidateCache()
        {
            lock (_lock)
            {
                _aliasMap = null;
            }
        }
    }

    /// <summary>
    /// Wynik preprocessingu tekstu oferty pracy.
    /// </summary>
    public class PreprocessedJobText
    {
        /// <summary>
        /// Oryginalny tekst oferty.
        /// </summary>
        public string OriginalText { get; set; } = "";

        /// <summary>
        /// Znormalizowany tekst z zamienionymi aliasami na nazwy kanoniczne.
        /// </summary>
        public string NormalizedText { get; set; } = "";

        /// <summary>
        /// Lista wykrytych skilli (kanoniczne nazwy).
        /// </summary>
        public List<string> DetectedSkills { get; set; } = new();

        /// <summary>
        /// Tokeny po preprocessingu.
        /// </summary>
        public List<string> Tokens { get; set; } = new();
    }
}
