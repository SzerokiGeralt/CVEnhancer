namespace CVEnhancer;

public partial class GeneratePage : ContentPage
{
	public GeneratePage()
	{
		InitializeComponent();
	}

	private async void OnAttachPdfClicked(object sender, EventArgs e)
	{
		// Placeholder dla funkcjonalnoœci za³¹czania PDF
		await DisplayAlert("Info", "Funkcjonalnoœæ za³¹czania PDF - do implementacji", "OK");
	}

	private async void OnAnalyzeClicked(object sender, EventArgs e)
	{
		// Placeholder dla funkcjonalnoœci analizy
		var jobDescription = JobDescriptionEditor.Text;
		
		if (string.IsNullOrWhiteSpace(jobDescription))
		{
			await DisplayAlert("B³¹d", "Wklej og³oszenie lub za³¹cz PDF", "OK");
			return;
		}
		
		await DisplayAlert("Info", "Analiza og³oszenia - do implementacji", "OK");
	}
}