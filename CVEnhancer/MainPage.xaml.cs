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
        public string FirstName => User.FirstName;
        public string LastName => User.LastName;
        public string FullName => User.FirstName + " " + LastName;
        public ImageSource ProfileImageSource => ImageByteConverter.ByteArrayToImageSource(User.ProfilePicture.Picture);
    }

    public partial class MainPage : ContentPage
    {
        public ObservableCollection<UserViewModel> UserProfiles { get; set; } = new();

        private readonly LocalDbService _localDbService;

        public MainPage(LocalDbService localDbService)
        {
            InitializeComponent();

            _localDbService = localDbService;

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
    }
}
