using CVEnhancer.Services;
namespace CVEnhancer;


public partial class GeneratePage : ContentPage
{
    private readonly AnalysisService _analysis;
    public GeneratePage(AnalysisService analysis)
	{
		InitializeComponent();
        _analysis = analysis;
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

        try
        {
            var result = await _analysis.AnalyzeJobOfferAsync(jobDescription);

            // MVP: poka¿ w Alert najwa¿niejsze info
            var msg =
                $"Overall: {result.OverallScore:0.00}\n\n" +
                $"Top projekty:\n- {string.Join("\n- ", result.TopProjects.Select(p => $"{p.Title} ({p.Score:0.00})"))}\n\n" +
                $"Top doœwiadczenia:\n- {string.Join("\n- ", result.TopWorkExperiences.Select(w => $"{w.Title} ({w.Score:0.00})"))}";

            await DisplayAlert("Wynik dopasowania", msg, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d analizy", ex.Message, "OK");
        }
    }
}