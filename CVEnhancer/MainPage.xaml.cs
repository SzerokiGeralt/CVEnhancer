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

            BindingContext = this;

            LoadUsers();
            _sessionService = sessionService;
        }

        private async void LoadUsers()
        {
            var users = await _localDbService.GetUsersWithPictures();

            foreach (var user in users)
            {
                UserProfiles.Add(new UserViewModel { User = user });
            }
        }

        private void Profile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int userId = (e.CurrentSelection.First() as UserViewModel).UserId;
                SetActiveProfile(userId);
            }
            catch (Exception ex) {
                DisplayAlert("Błąd","Wybrano nieprawidłowy profil" + ex.Message,"OK");
                return;
            }
            DisplayAlert("Zalogowano poprawnie", (e.CurrentSelection.First() as UserViewModel)?.FullName, "OK");
        }

        private async void SetActiveProfile(int userId) {
            var x = await _localDbService.GetUserById(userId);
            _sessionService.Login(x);
        }
    }
}
