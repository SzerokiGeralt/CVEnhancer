using CVEnhancer.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CVEnhancer
{
    // 1. Model danych (Prosty)
    public class UserProfile
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        // Kolekcja profili widoczna dla XAMLa
        public ObservableCollection<UserProfile> Profiles { get; set; } = new();

        public MainPage(LocalDbService localDbService)
        {
            InitializeComponent();

            // Ustawiamy ten plik jako źródło danych dla samego siebie
            BindingContext = this;

            // Ładowanie przykładowych danych (Fake Data)
            LoadProfiles();
        }

        private void LoadProfiles()
        {
            Profiles.Add(new UserProfile { Name = "Jan Kowalski", ImageUrl = "https://picsum.photos/200" });
            Profiles.Add(new UserProfile { Name = "Anna Nowak", ImageUrl = "https://picsum.photos/201" });
            Profiles.Add(new UserProfile { Name = "Piotr Wiśniewski", ImageUrl = "https://picsum.photos/202" });
            // Profiles.Add(new UserProfile { Name = "Maria Wójcik", ImageUrl = "https://picsum.photos/203" });
        }
    }
}
