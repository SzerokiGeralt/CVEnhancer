using System.Windows.Input;
using CVEnhancer.Services;

namespace CVEnhancer
{
    public partial class AppShell : Shell
    {
        private readonly SessionService _sessionService;
        public ICommand GoToRouteCommand { get; }
        public ICommand LogoutCommand { get; }

        public string UserDisplayName => _sessionService.ActiveUser != null 
            ? $"{_sessionService.ActiveUser.FirstName} {_sessionService.ActiveUser.LastName}"
            : "Gość";

        public AppShell(SessionService sessionService)
        {
            InitializeComponent();
            
            _sessionService = sessionService;

            // Komenda nawigacji
            GoToRouteCommand = new Command<string>(async (route) =>
            {
                await Current.GoToAsync(route);
            });

            // Komenda wylogowania
            LogoutCommand = new Command(async () =>
            {
                _sessionService.Logout();
                
                // Ukryj flyout po wylogowaniu
                FlyoutBehavior = FlyoutBehavior.Disabled;
                
                await Current.GoToAsync("//LoginPage");
            });

            // Nasłuchuj zmian w autentykacji
            _sessionService.AuthenticationChanged += OnAuthenticationChanged;

            // Ukryj flyout na starcie (przed zalogowaniem)
            FlyoutBehavior = FlyoutBehavior.Disabled;

            BindingContext = this;
        }

        private void OnAuthenticationChanged(object? sender, bool isAuthenticated)
        {
            // Odśwież nazwę użytkownika
            OnPropertyChanged(nameof(UserDisplayName));
            
            // Pokaż/ukryj flyout w zależności od stanu logowania
            FlyoutBehavior = isAuthenticated ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
        }
    }
}
