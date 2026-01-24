using CVEnhancer.DTO;
using CVEnhancer.Services;
using CVEnhancer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CVEnhancer.ViewModels
{
    public class CvPreviewViewModel
    {
        private readonly SessionService _session;
        private readonly PdfExportService _pdf;
        private readonly List<MatchedItemDTO> _selected;
        // ===== Personal =====
        public string FullName { get; }
        public string Headline { get; }
        public string Summary { get; }

        public bool HasSummary => !string.IsNullOrWhiteSpace(Summary);
        public bool ShowSummaryHint => !HasSummary;
        public string SummaryHint => "Brak podsumowania zawodowego — możesz je uzupełnić w zakładce Dane.";

        public string Email { get; }
        public string Phone { get; }
        public string LinkedIn { get; }
        public string GitHub { get; }
        public string Portfolio { get; }

        public bool HasEmail => !string.IsNullOrWhiteSpace(Email);
        public bool HasPhone => !string.IsNullOrWhiteSpace(Phone);
        public bool HasLinkedIn => !string.IsNullOrWhiteSpace(LinkedIn);
        public bool HasGitHub => !string.IsNullOrWhiteSpace(GitHub);
        public bool HasPortfolio => !string.IsNullOrWhiteSpace(Portfolio);

        public ImageSource? ProfileImageSource { get; }

        // ===== Sections =====
        public ObservableCollection<CvItemRowVM> WorkItems { get; }
        public ObservableCollection<CvItemRowVM> ProjectItems { get; }
        public ObservableCollection<CvItemRowVM> CertificateItems { get; }
        public ObservableCollection<CvItemRowVM> EducationItems { get; }
        public ObservableCollection<string> Skills { get; }
        public bool HasWork => WorkItems.Count > 0;
        public bool HasProjects => ProjectItems.Count > 0;
        public bool HasCertificates => CertificateItems.Count > 0;
        public bool HasEducation => EducationItems.Count > 0;
        public bool HasSkills => Skills.Count > 0;
        public string WorkHeader => "Doświadczenie zawodowe";
        public string ProjectsHeader => "Projekty";
        public string CertificatesHeader => "Certyfikaty";
        public string EducationHeader => "Edukacja";
        public string SkillsHeader => "Umiejętności i narzędzia";

        // ===== Commands =====
        public ICommand BackCommand { get; }
        public ICommand ExportCommand { get; }

        public CvPreviewViewModel(SessionService session, PdfExportService pdf, List<MatchedItemDTO> selected)
        {
            _session = session;
            _pdf = pdf;
            _selected = selected;

            var user = _session.ActiveUser ?? throw new InvalidOperationException("Brak zalogowanego użytkownika.");

            FullName = $"{user.FirstName} {user.LastName}";
            Headline = string.IsNullOrWhiteSpace(user.JobTitle) ? "—" : user.JobTitle!;
            Summary = user.ProfessionalSummary ?? "";

            Email = user.Email ?? "";
            Phone = user.PhoneNumber ?? "";
            LinkedIn = user.LinkedInUrl ?? "";
            GitHub = user.GitHubUrl ?? "";
            Portfolio = user.PortfolioUrl ?? "";

            ProfileImageSource = user.ProfilePicture?.Picture != null
                ? ImageByteConverter.ByteArrayToImageSource(user.ProfilePicture.Picture)
                : null;

            // grupowanie zaznaczonych elementów po typie
            var work = selected.Where(x => x.Type == "WorkExperience").ToList();
            var proj = selected.Where(x => x.Type == "Project").ToList();
            var cert = selected.Where(x => x.Type == "Certificate").ToList();
            var edu = selected.Where(x => x.Type == "Education").ToList();
            var skills = selected
                .SelectMany(x => x.MatchedSkills ?? new List<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();
            // Podgląd CV: proste VM bez selekcji i bez score
            WorkItems = new ObservableCollection<CvItemRowVM>(work.Select(x => new CvItemRowVM(x)));
            ProjectItems = new ObservableCollection<CvItemRowVM>(proj.Select(x => new CvItemRowVM(x)));
            CertificateItems = new ObservableCollection<CvItemRowVM>(cert.Select(x => new CvItemRowVM(x)));
            EducationItems = new ObservableCollection<CvItemRowVM>(edu.Select(x => new CvItemRowVM(x)));
            Skills = new ObservableCollection<string>(skills);

            BackCommand = new Command(async () =>
            {
                if (Shell.Current?.Navigation != null)
                    await Shell.Current.Navigation.PopAsync();
            });

            ExportCommand = new Command(async () =>
            {
                try
                {
                    var pdfBytes = _pdf.GenerateCvPdfBytes(_selected);

                    var fileName = $"CV_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    var path = Path.Combine(FileSystem.CacheDirectory, fileName);
                    File.WriteAllBytes(path, pdfBytes);

                    await Application.Current!.MainPage.DisplayAlert("PDF gotowy", $"Zapisano:\n{path}", "OK");

                    // Opcjonalnie: udostępnij / otwórz
                    // await Share.Default.RequestAsync(new ShareFileRequest
                    // {
                    //     Title = "Udostępnij CV",
                    //     File = new ShareFile(path)
                    // });
                }
                catch (Exception ex)
                {
                    await Application.Current!.MainPage.DisplayAlert("Błąd eksportu", ex.Message, "OK");
                }
            });
        }
    }

    /// <summary>
    /// Prosty VM do wyświetlania elementu w podglądzie CV (bez selekcji).
    /// </summary>
    public class CvItemRowVM
    {
        public MatchedItemDTO Dto { get; }

        public CvItemRowVM(MatchedItemDTO dto)
        {
            Dto = dto;
        }

        public string Title => Dto.Title;
        public string Description => Dto.Description;

    }
}
