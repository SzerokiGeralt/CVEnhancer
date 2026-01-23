using CVEnhancer.DTO;
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

        await DisplayAlert("Wybrane elementy",
            $"Zaznaczono: {selected.Count}\n\n" +
            string.Join("\n", selected.Take(10).Select(x => $"• [{x.Type}] {x.Title}")),
            "OK");
    }
}
