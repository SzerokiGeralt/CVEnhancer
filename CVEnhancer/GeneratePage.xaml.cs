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
        await DisplayAlert("Info", "Funkcjonalnoœæ za³¹czania PDF - do implementacji", "OK");
    }

    private async void OnAnalyzeClicked(object sender, EventArgs e)
    {
        var jobDescription = JobDescriptionEditor.Text;
        
        if (string.IsNullOrWhiteSpace(jobDescription))
        {
            await DisplayAlert("B³¹d", "Wklej og³oszenie lub za³¹cz PDF", "OK");
            return;
        }

        try
        {
            // Poka¿ loading overlay i zablokuj przycisk
            SetLoadingState(true);

            // Wykonaj analizê (ju¿ jest async, wiêc nie blokuje UI)
            var result = await _analysis.AnalyzeJobOfferAsync(jobDescription);

            // Ukryj loading
            SetLoadingState(false);

            // Formatuj wynik
            var skillsList = result.DetectedJobSkills.Count > 0 
                ? string.Join(", ", result.DetectedJobSkills.Take(10)) 
                : "Brak";
            
            var keywordsList = result.JobKeywords.Count > 0 
                ? string.Join(", ", result.JobKeywords.Take(10)) 
                : "Brak";

            var topMatchesList = result.TopMatches.Count > 0
                ? string.Join("\n", result.TopMatches.Take(5).Select(m => 
                    $"• [{m.Type}] {m.Title}: {m.Score:P0}"))
                : "Brak dopasowañ";

            var msg =
                $"Ogólne dopasowanie: {result.OverallScore:P0}\n\n" +
                $"Wykryte skille ({result.DetectedJobSkills.Count}):\n{skillsList}\n\n" +
                $"S³owa kluczowe:\n{keywordsList}\n\n" +
                $"Najlepsze dopasowania:\n{topMatchesList}";

            await DisplayAlert("Wynik dopasowania", msg, "OK");
        }
        catch (Exception ex)
        {
            SetLoadingState(false);
            await DisplayAlert("B³¹d analizy", ex.Message, "OK");
        }
    }

    private void SetLoadingState(bool isLoading)
    {
        LoadingOverlay.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
        AnalyzeButton.IsEnabled = !isLoading;
    }
}