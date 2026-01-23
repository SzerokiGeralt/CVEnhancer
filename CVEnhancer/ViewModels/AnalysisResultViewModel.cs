using CVEnhancer.DTO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CVEnhancer.ViewModels
{
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

        // Te kolekcje są bindowane do SelectedItems w XAML
        public ObservableCollection<MatchedItemRowVM> SelectedWorkItems { get; } = new();
        public ObservableCollection<MatchedItemRowVM> SelectedProjectItems { get; } = new();
        public ObservableCollection<MatchedItemRowVM> SelectedCertificateItems { get; } = new();
        public ObservableCollection<MatchedItemRowVM> SelectedEducationItems { get; } = new();

        public ICommand ToggleSelectCommand { get; }


        public AnalysisResultViewModel(MatchingResultDTO result)
        {
            ToggleSelectCommand = new Command<MatchedItemRowVM>(item =>
            {
                if (item == null) return;
                item.IsSelected = !item.IsSelected;
            });

            OverallScoreText = $"{result.OverallScore:P0}";

            DetectedSkillsText =
                $"Wykryte skille ({result.DetectedJobSkills.Count}): " +
                (result.DetectedJobSkills.Count == 0
                    ? "Brak"
                    : string.Join(", ", result.DetectedJobSkills));

            KeywordsText =
                result.JobKeywords.Count == 0
                    ? "Słowa kluczowe: Brak"
                    : $"Słowa kluczowe: {string.Join(", ", result.JobKeywords.Take(25))}";

            var work = GetList(result, "WorkExperience");
            var proj = GetList(result, "Project");
            var cert = GetList(result, "Certificate");
            var edu = GetList(result, "Education");

            WorkHeader = $"Doświadczenie ({work.Count})";
            ProjectsHeader = $"Projekty ({proj.Count})";
            CertificatesHeader = $"Certyfikaty ({cert.Count})";
            EducationHeader = $"Edukacja ({edu.Count})";

            var all = work.Concat(proj).Concat(cert).Concat(edu).ToList();
            var bestRaw = all.Count == 0 ? 0.0 : all.Max(x => x.Score);

            // bestRaw ~ 0.15 -> t ~ 0, ale weight nadal > 0 (minimalna ulga)
            double t = Clamp01((bestRaw - 0.12) / (0.55 - 0.12)); // 0..1
            double weight = 0.20 + 0.80 * t;                      // 0.20..1.00
            double liftStrength = 0.55;                           // mocniej, bo best jest zwykle niskie


            WorkItems = new ObservableCollection<MatchedItemRowVM>(
                work.Select(x => new MatchedItemRowVM(x, bestRaw, weight, liftStrength)));

            ProjectItems = new ObservableCollection<MatchedItemRowVM>(
                proj.Select(x => new MatchedItemRowVM(x, bestRaw, weight, liftStrength)));

            CertificateItems = new ObservableCollection<MatchedItemRowVM>(
                cert.Select(x => new MatchedItemRowVM(x, bestRaw, weight, liftStrength)));

            EducationItems = new ObservableCollection<MatchedItemRowVM>(
                edu.Select(x => new MatchedItemRowVM(x, bestRaw, weight, liftStrength)));
        }

        private static List<MatchedItemDTO> GetList(MatchingResultDTO result, string type)
            => result.MatchesByType.TryGetValue(type, out var list) ? list : new List<MatchedItemDTO>();

        private static double Clamp01(double v) => v < 0 ? 0 : (v > 1 ? 1 : v);
        /// <summary>
        /// Zwraca wszystkie zaznaczone elementy z każdej sekcji.
        /// </summary>
        public List<MatchedItemDTO> GetAllSelectedDtos()
        {
            return WorkItems.Where(x => x.IsSelected).Select(x => x.Dto)
                .Concat(ProjectItems.Where(x => x.IsSelected).Select(x => x.Dto))
                .Concat(CertificateItems.Where(x => x.IsSelected).Select(x => x.Dto))
                .Concat(EducationItems.Where(x => x.IsSelected).Select(x => x.Dto))
                .ToList();
        }
    }

    public class MatchedItemRowVM : INotifyPropertyChanged
    {
        public MatchedItemDTO Dto { get; }

        public double RawScore { get; }
        public double DisplayScore { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public MatchedItemRowVM(MatchedItemDTO dto, double bestRaw, double weight, double liftStrength)
        {
            Dto = dto;
            RawScore = dto.Score;

            if (bestRaw <= 0)
            {
                DisplayScore = 0;
            }
            else
            {
                var normalized = RawScore / bestRaw; // 0..1
                var display = RawScore + weight * normalized * (1 - bestRaw) * liftStrength;
                DisplayScore = display < 0 ? 0 : (display > 1 ? 1 : display);
            }
        }

        public string Title => Dto.Title;
        public string Description => Dto.Description;

        // ✅ teraz pokazujemy "ulgowy" wynik
        public string ScoreText => $"{DisplayScore:P0}";

        public string MatchedSkillsText =>
            Dto.MatchedSkills.Count == 0
                ? "Dopasowane skille: —"
                : $"Dopasowane skille: {string.Join(", ", Dto.MatchedSkills)}";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}
