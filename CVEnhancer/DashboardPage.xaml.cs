using CVEnhancer.Services;

namespace CVEnhancer;

public partial class DashboardPage : ContentPage
{
	private SessionService _sessionService;
	public string FirstName { get; set; } = string.Empty;
    public DashboardPage(LocalDbService localDbService, SessionService sessionService)
	{
        InitializeComponent();
        _sessionService = sessionService;
		FirstName = _sessionService.ActiveUser.FirstName;
        BindingContext = this;

    }

	private async void OnGoToLibraryClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//LibraryPage");
	}

	private async void OnGoToGenerateClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//GeneratePage");
	}
}