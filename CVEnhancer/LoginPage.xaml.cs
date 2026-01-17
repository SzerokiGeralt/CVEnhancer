using CVEnhancer.Models;
using CVEnhancer.Services;
using CVEnhancer.Utils;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CVEnhancer
{
    public class UserViewModel
    {
        public User User { get; set; }
        public int UserId => User.UserId;
        public string FirstName => User.FirstName;
        public string LastName => User.LastName;
        public string FullName => User.FirstName + " " + LastName;
        public ImageSource ProfileImageSource => ImageByteConverter.ByteArrayToImageSource(User.ProfilePicture.Picture);
    }

    public partial class MainPage : ContentPage
    {
        public ObservableCollection<UserViewModel> UserProfiles { get; set; } = new();

        private readonly LocalDbService _localDbService;
        private readonly SessionService _sessionService;

        public MainPage(LocalDbService localDbService, SessionService sessionService)
        {
            InitializeComponent();

            _localDbService = localDbService;
            _sessionService = sessionService;

            BindingContext = this;

            LoadUsers();
        }

        private async void LoadUsers()
        {
            var users = await _localDbService.GetUsersWithPictures();

            foreach (var user in users)
            {
                UserProfiles.Add(new UserViewModel { User = user });
            }
        }

        private async void Profile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.CurrentSelection.Count == 0) return;
                
                int userId = (e.CurrentSelection.First() as UserViewModel).UserId;
                await SetActiveProfile(userId);
                
                await DisplayAlert("Zalogowano poprawnie", (e.CurrentSelection.First() as UserViewModel)?.FullName, "OK");
                
                // Po zalogowaniu przekieruj do Dashboard
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            catch (Exception ex) 
            {
                await DisplayAlert("Błąd", "Wybrano nieprawidłowy profil: " + ex.Message, "OK");
                return;
            }
        }

        private async Task SetActiveProfile(int userId) 
        {
            var user = await _localDbService.GetUserById(userId);
            _sessionService.Login(user);
        }
    }
}
