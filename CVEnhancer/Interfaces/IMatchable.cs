namespace CVEnhancer.Interfaces
{
    /// <summary>
    /// Interfejs dla obiektów, które mog¹ byæ dopasowywane do ofert pracy.
    /// </summary>
    public interface IMatchable
    {
        /// <summary>
        /// Unikalny identyfikator obiektu.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Typ obiektu (np. "Project", "WorkExperience", "Certificate", "Education").
        /// </summary>
        string MatchableType { get; }

        /// <summary>
        /// Tytu³/nazwa do wyœwietlenia w UI.
        /// </summary>
        string DisplayTitle { get; }

        /// <summary>
        /// Opis lub dodatkowe informacje do wyœwietlenia.
        /// </summary>
        string DisplayDescription { get; }

        /// <summary>
        /// Tekst do analizy TF-IDF (nazwa, opis, skille itp.).
        /// </summary>
        string GetSearchableText();

        /// <summary>
        /// Lista nazw powi¹zanych skilli.
        /// </summary>
        IEnumerable<string> GetSkillNames();
    }
}
