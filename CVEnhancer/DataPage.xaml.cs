using CVEnhancer.Services;
using CVEnhancer.ViewModels;
using System.Threading.Tasks;

namespace CVEnhancer;

public partial class DataPage : ContentPage
{
    private DataPageViewModel _viewModel;
    public DataPage(LocalDbService db, SessionService session)
    {
        InitializeComponent();
        _viewModel = new DataPageViewModel(db, session);
        BindingContext = _viewModel;
        ShowPersonalData();
    }

    private void OnPersonalDataClicked(object sender, EventArgs e) => ShowPersonalData();
    private void OnWorkClicked(object sender, EventArgs e) => ShowSection(WorkContent);
    private void OnEducationClicked(object sender, EventArgs e) => ShowSection(EducationContent);
    private void OnProjectsClicked(object sender, EventArgs e) => ShowSection(ProjectsContent);
    private void OnCertificatesClicked(object sender, EventArgs e) => ShowSection(CertificatesContent);

    private void ShowPersonalData() => ShowSection(PersonalDataContent);

    private void ShowSection(VerticalStackLayout section)
    {
        PersonalDataContent.IsVisible = false;
        WorkContent.IsVisible = false;
        EducationContent.IsVisible = false;
        ProjectsContent.IsVisible = false;
        CertificatesContent.IsVisible = false;
        section.IsVisible = true;
    }
    private async void DeleteProfileClicked(object sender, EventArgs e)
    {
        var choice = await DisplayActionSheet("Czy na pewno chcesz usun¹æ profil?", "Anuluj", "Usuñ");
        if (choice == "Usuñ")
        {
            _viewModel.DeleteProfileCommand.Execute(null);
            await DisplayAlert("Uwaga", "Poprawnie usuniêto profil u¿ytkownika", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }

    private async void SaveClicked(object sender, EventArgs e)
    {
        try
        {
            _viewModel.SavePersonalDataCommand.Execute(null);
            await DisplayAlert("Uwaga", "Poprawnie zapisano dane osobowe", "OK");
        }
        catch (Exception ex) {
            await DisplayAlert("B³¹d zapisywania", ex.Message, "OK");
        }
    }
}