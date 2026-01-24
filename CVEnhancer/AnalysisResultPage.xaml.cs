using CVEnhancer.DTO;
using CVEnhancer.Services;
using CVEnhancer.ViewModels;

namespace CVEnhancer;

public partial class AnalysisResultPage : ContentPage
{
    public AnalysisResultPage(MatchingResultDTO result)
    {
        InitializeComponent();
        BindingContext = new AnalysisResultViewModel(result);
    }

    private async void OnGenerateCvClicked(object sender, EventArgs e)
    {
        var vm = (AnalysisResultViewModel)BindingContext;
        var selected = vm.GetAllSelectedDtos();

        if (selected.Count == 0)
        {
            await DisplayAlert("Brak zaznaczeñ", "Zaznacz przynajmniej jeden kafelek.", "OK");
            return;
        }

        var services = Application.Current?.Handler?.MauiContext?.Services;

        var session = services?.GetService<SessionService>();
        if (session?.ActiveUser == null)
        {
            await DisplayAlert("B³¹d", "Brak zalogowanego u¿ytkownika.", "OK");
            return;
        }

        var pdf = services?.GetService<PdfExportService>();
        if (pdf == null)
        {
            await DisplayAlert("B³¹d", "Brak PdfExportService (sprawdŸ rejestracjê w MauiProgram).", "OK");
            return;
        }

        await Shell.Current.Navigation.PushAsync(new CvPreviewPage(session, pdf, selected));

    }
}
