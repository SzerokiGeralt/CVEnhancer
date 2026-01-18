using CVEnhancer.Models;
using CVEnhancer.Services;
namespace CVEnhancer;


public partial class DataPage : ContentPage
{
    private readonly ProfileService _profileService;
    private readonly SessionService _sessionService;

    private User? _currentUser;
    public DataPage(SessionService sessionService, ProfileService profileService)
    {
        InitializeComponent();
        _sessionService = sessionService;
        _profileService = profileService;
        ShowPersonalData();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadPersonalDataAsync();
        await LoadWorkExperiencesAsync();
    }

    private async Task LoadPersonalDataAsync()
    {
        if (_sessionService?.ActiveUser == null)
        {
            // Mo¿esz tu przerzuciæ na stronê logowania jeœli chcesz
            return;
        }

        // pobierz œwie¿ego usera z DB (¿eby mieæ aktualne dane)
        _currentUser = await _profileService.GetUserAsync(_sessionService.ActiveUser.UserId);
        if (_currentUser == null) return;

        FirstNameEntry.Text = _currentUser.FirstName;
        LastNameEntry.Text = _currentUser.LastName;
        EmailEntry.Text = _currentUser.Email;
        PhoneEntry.Text = _currentUser.PhoneNumber;
        LinkedInEntry.Text = _currentUser.LinkedInUrl;
        GitHubEntry.Text = _currentUser.GitHubUrl;
        PortfolioEntry.Text = _currentUser.PortfolioUrl;
        JobTitleEntry.Text = _currentUser.JobTitle;
        SummaryEditor.Text = _currentUser.ProfessionalSummary;
    }

    private async void OnSavePersonalDataClicked(object sender, EventArgs e)
    {
        if (_currentUser == null)
            await LoadPersonalDataAsync();

        if (_currentUser == null)
        {
            await DisplayAlert("B³¹d", "Brak aktywnego u¿ytkownika.", "OK");
            return;
        }

        // aktualizujemy TEN SAM rekord (Id musi zostaæ)
        var updated = new User
        {
            UserId = _currentUser.UserId,
            FirstName = FirstNameEntry.Text?.Trim(),
            LastName = LastNameEntry.Text?.Trim(),
            Email = EmailEntry.Text?.Trim(),
            PhoneNumber = PhoneEntry.Text?.Trim(),
            LinkedInUrl = LinkedInEntry.Text?.Trim(),
            GitHubUrl = GitHubEntry.Text?.Trim(),
            PortfolioUrl = PortfolioEntry.Text?.Trim(),
            JobTitle = JobTitleEntry.Text?.Trim(),
            ProfessionalSummary = SummaryEditor.Text?.Trim()
        };

        // minimalna walidacja
        if (string.IsNullOrWhiteSpace(updated.FirstName) || string.IsNullOrWhiteSpace(updated.LastName))
        {
            await DisplayAlert("Walidacja", "Imiê i nazwisko nie mog¹ byæ puste.", "OK");
            return;
        }

        await _profileService.UpdateUserAsync(updated);

        // odœwie¿ formularz + sesjê
        _currentUser = await _profileService.GetUserAsync(updated.UserId);
        if (_currentUser != null)
        {
            _sessionService.Login(_currentUser); // odœwie¿a ActiveUser (logicznie to update sesji)
            await DisplayAlert("Sukces", "Zapisano dane profilu.", "OK");
        }
    }
    private void OnPersonalDataClicked(object sender, EventArgs e)
	{
		ShowPersonalData();
	}

    private async Task LoadWorkExperiencesAsync()
    {
        if (_sessionService.ActiveUser == null) return;

        var items = await _profileService.GetWorkExperiencesAsync(_sessionService.ActiveUser.UserId);
        WorkExperienceList.ItemsSource = items;
    }

    private async void OnAddWorkExperienceClicked(object sender, EventArgs e)
    {
        if (_sessionService.ActiveUser == null)
        {
            await DisplayAlert("B³¹d", "Brak aktywnego u¿ytkownika.", "OK");
            return;
        }

        var company = CompanyEntry.Text?.Trim();
        var title = WorkJobTitleEntry.Text?.Trim();
        var desc = WorkDescriptionEditor.Text?.Trim();

        if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(title))
        {
            await DisplayAlert("Walidacja", "Firma i stanowisko s¹ wymagane.", "OK");
            return;
        }

        DateTime? end = EndDatePicker.Date;
        if (end.Value.Date < StartDatePicker.Date)
            end = null;

        var work = new WorkExperience
        {
            CompanyName = company,
            JobTitle = title,
            StartDate = StartDatePicker.Date,
            EndDate = end,
            Description = desc ?? ""
        };

        await _profileService.AddWorkExperienceAsync(_sessionService.ActiveUser.UserId, work);

        // wyczyœæ formularz
        CompanyEntry.Text = "";
        WorkJobTitleEntry.Text = "";
        WorkDescriptionEditor.Text = "";
        StartDatePicker.Date = DateTime.Today;
        EndDatePicker.Date = DateTime.Today;

        await LoadWorkExperiencesAsync();
    }

    private async void OnDeleteWorkExperienceClicked(object sender, EventArgs e)
    {
        if (_sessionService.ActiveUser == null) return;

        if (sender is Button btn && btn.CommandParameter is int workId)
        {
            bool confirm = await DisplayAlert("PotwierdŸ", "Usun¹æ wpis?", "Tak", "Nie");
            if (!confirm) return;

            await _profileService.DeleteWorkExperienceAsync(_sessionService.ActiveUser.UserId, workId);
            await LoadWorkExperiencesAsync();
        }
    }
    private async void OnWorkClicked(object sender, EventArgs e)
	{
		HideAllContent();
		WorkContent.IsVisible = true;
        await LoadWorkExperiencesAsync();
    }

	private void OnEducationClicked(object sender, EventArgs e)
	{
		HideAllContent();
		EducationContent.IsVisible = true;
	}

	private void OnProjectsClicked(object sender, EventArgs e)
	{
		HideAllContent();
		ProjectsContent.IsVisible = true;
	}

	private void ShowPersonalData()
	{
		HideAllContent();
		PersonalDataContent.IsVisible = true;
	}

	private void HideAllContent()
	{
		PersonalDataContent.IsVisible = false;
		WorkContent.IsVisible = false;
		EducationContent.IsVisible = false;
		ProjectsContent.IsVisible = false;
	}
}