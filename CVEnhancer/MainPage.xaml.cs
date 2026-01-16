using CVEnhancer.Services;
using System.Threading.Tasks;

namespace CVEnhancer
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage(LocalDbService localDbService)
        {
            InitializeComponent();
            localDbService.AddUser().Wait();
        }


        private async void OnCounterClicked(object? sender, EventArgs e)
        {
            await DisplayAlert("Baza jest w", FileSystem.AppDataDirectory , "OK");
        }
    }
}
