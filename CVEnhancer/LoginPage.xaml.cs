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
                var user = await _localDbService.GetUserById(userId);
                _sessionService.Login(user);

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

        private async void AddNewProfileButton(object sender, EventArgs e)
        {
            int userCount = await _localDbService.CountUsers();
            if (userCount >= 5)
            {
                await DisplayAlert("Błąd", "Osiągnięto maksymalną liczbę profili", "OK");
                return;
            }
            else
            {
                try
                {
                    var firstName = await DisplayPromptAsync("Podaj imię", "Podaj imię", "OK");
                    var lastName = await DisplayPromptAsync("Podaj nazwisko", "Podaj nazwisko", "OK");
                    var x = new User() { FirstName = firstName, LastName = lastName, ProfilePicture = new ProfilePicture() { Picture = ImageByteConverter.defaultImage }, };
                    await _localDbService.AddUser(x);
                    _sessionService.Login(x);

                    var y = new UserViewModel { User = x };
                    await DisplayAlert("Zalogowano poprawnie", y.FullName, "OK");
                    await DisplayAlert("Uzupełnij resztę swoich danych", y.FullName, "OK");

                    await Shell.Current.GoToAsync("//DataPage");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Błąd", ex.Message, "OK");
                    return;
                }
            }
        }
    }
}
