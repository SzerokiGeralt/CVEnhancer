using CVEnhancer.DTO;
using System.Collections.ObjectModel;

namespace CVEnhancer;

public partial class AnalysisResultPage : ContentPage
{
    public AnalysisResultPage(MatchingResultDTO result)
    {
        InitializeComponent();
        BindingContext = new AnalysisResultViewModel(result);
    }
}

public class AnalysisResultViewModel
{
    public string OverallScoreText { get; }
    public string DetectedSkillsText { get; }
    public string KeywordsText { get; }

    public string WorkHeader { get; }
    public string ProjectsHeader { get; }
    public string CertificatesHeader { get; }
    public string EducationHeader { get; }

    public ObservableCollection<MatchedItemRowVM> WorkItems { get; }
    public ObservableCollection<MatchedItemRowVM> ProjectItems { get; }
    public ObservableCollection<MatchedItemRowVM> CertificateItems { get; }
    public ObservableCollection<MatchedItemRowVM> EducationItems { get; }

    public AnalysisResultViewModel(MatchingResultDTO result)
    {
        OverallScoreText = $"Ogólne dopasowanie: {result.OverallScore:P0}";
        DetectedSkillsText = $"Wykryte skille ({result.DetectedJobSkills.Count}): " +
                             (result.DetectedJobSkills.Count == 0 ? "Brak" : string.Join(", ", result.DetectedJobSkills));
        KeywordsText = $"S³owa kluczowe: " +
                       (result.JobKeywords.Count == 0 ? "Brak" : string.Join(", ", result.JobKeywords.Take(25)));

        var work = GetList(result, "WorkExperience");
        var proj = GetList(result, "Project");
        var cert = GetList(result, "Certificate");
        var edu = GetList(result, "Education");

        WorkHeader = $"Doœwiadczenie ({work.Count})";
        ProjectsHeader = $"Projekty ({proj.Count})";
        CertificatesHeader = $"Certyfikaty ({cert.Count})";
        EducationHeader = $"Edukacja ({edu.Count})";

        WorkItems = new ObservableCollection<MatchedItemRowVM>(work.Select(x => new MatchedItemRowVM(x)));
        ProjectItems = new ObservableCollection<MatchedItemRowVM>(proj.Select(x => new MatchedItemRowVM(x)));
        CertificateItems = new ObservableCollection<MatchedItemRowVM>(cert.Select(x => new MatchedItemRowVM(x)));
        EducationItems = new ObservableCollection<MatchedItemRowVM>(edu.Select(x => new MatchedItemRowVM(x)));
    }

    private static List<MatchedItemDTO> GetList(MatchingResultDTO result, string type)
        => result.MatchesByType.TryGetValue(type, out var list) ? list : new List<MatchedItemDTO>();
}

public class MatchedItemRowVM
{
    private readonly MatchedItemDTO _dto;

    public MatchedItemRowVM(MatchedItemDTO dto) => _dto = dto;

    public string Title => _dto.Title;
    public string Description => _dto.Description;
    public string ScoreText => $"Dopasowanie: {_dto.Score:P0}";
    public string MatchedSkillsText =>
        _dto.MatchedSkills.Count == 0
            ? "Dopasowane skille: —"
            : $"Dopasowane skille: {string.Join(", ", _dto.MatchedSkills)}";
}
