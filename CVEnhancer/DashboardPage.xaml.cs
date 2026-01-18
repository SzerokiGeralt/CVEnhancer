namespace CVEnhancer;

public partial class DashboardPage : ContentPage
{
	public DashboardPage()
	{
		InitializeComponent();
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