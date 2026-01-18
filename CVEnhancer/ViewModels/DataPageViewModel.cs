using CVEnhancer.Models;
using CVEnhancer.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CVEnhancer.ViewModels
{
    public class DataPageViewModel : INotifyPropertyChanged
    {
        private readonly LocalDbService _db;
        private readonly SessionService _session;

        // Aktualny u¿ytkownik (dane osobowe)
        public User CurrentUser => _session.ActiveUser!;

        // Kolekcje dla list
        public ObservableCollection<WorkExperience> WorkExperiences { get; } = new();
        public ObservableCollection<Education> Educations { get; } = new();
        public ObservableCollection<Project> Projects { get; } = new();
        public ObservableCollection<Certificate> Certificates { get; } = new();

        // ===== WORK EXPERIENCE =====
        private WorkExperience _currentWork = new() { StartDate = DateTime.Today };
        public WorkExperience CurrentWork
        {
            get => _currentWork;
            set { _currentWork = value; OnPropertyChanged(); OnPropertyChanged(nameof(WorkEndDate)); }
        }

        // Wrapper dla nullable EndDate
        public DateTime WorkEndDate
        {
            get => CurrentWork.EndDate ?? DateTime.Today;
            set => CurrentWork.EndDate = value;
        }

        // ===== EDUCATION =====
        private Education _currentEducation = new() { StartDate = DateTime.Today };
        public Education CurrentEducation
        {
            get => _currentEducation;
            set { _currentEducation = value; OnPropertyChanged(); OnPropertyChanged(nameof(EducationEndDate)); }
        }

        public DateTime EducationEndDate
        {
            get => CurrentEducation.EndDate ?? DateTime.Today;
            set => CurrentEducation.EndDate = value;
        }

        // ===== PROJECT =====
        private Project _currentProject = new();
        public Project CurrentProject
        {
            get => _currentProject;
            set { _currentProject = value; OnPropertyChanged(); }
        }

        // ===== CERTIFICATE =====
        private Certificate _currentCertificate = new() { IssueDate = DateTime.Today };
        public Certificate CurrentCertificate
        {
            get => _currentCertificate;
            set { _currentCertificate = value; OnPropertyChanged(); OnPropertyChanged(nameof(CertificateExpirationDate)); }
        }

        public DateTime CertificateExpirationDate
        {
            get => CurrentCertificate.ExpirationDate ?? DateTime.Today;
            set => CurrentCertificate.ExpirationDate = value;
        }

        // Komendy
        public ICommand SavePersonalDataCommand { get; }
        public ICommand AddWorkCommand { get; }
        public ICommand AddEducationCommand { get; }
        public ICommand AddProjectCommand { get; }
        public ICommand AddCertificateCommand { get; }
        public ICommand DeleteWorkCommand { get; }
        public ICommand DeleteEducationCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand DeleteCertificateCommand { get; }
        public ICommand DeleteProfileCommand { get; }

        public DataPageViewModel(LocalDbService db, SessionService session)
        {
            _db = db;
            _session = session;

            // Inicjalizacja komend
            SavePersonalDataCommand = new Command(async () => await SavePersonalData());
            DeleteProfileCommand = new Command(async () => await DeleteProfile());
            AddWorkCommand = new Command(async () => await AddWork());
            AddEducationCommand = new Command(async () => await AddEducation());
            AddProjectCommand = new Command(async () => await AddProject());
            AddCertificateCommand = new Command(async () => await AddCertificate());
            DeleteWorkCommand = new Command<WorkExperience>(async w => await DeleteWork(w));
            DeleteEducationCommand = new Command<Education>(async e => await DeleteEducation(e));
            DeleteProjectCommand = new Command<Project>(async p => await DeleteProject(p));
            DeleteCertificateCommand = new Command<Certificate>(async c => await DeleteCertificate(c));

            LoadData();
        }

        private async void LoadData()
        {
            if (_session.ActiveUser == null) return;

            var user = await _db.GetUserWithAllData(_session.ActiveUser.UserId);
            if (user == null) return;

            // Aktualizuj sesjê
            _session.ActiveUser.WorkExperiences = user.WorkExperiences;
            _session.ActiveUser.Educations = user.Educations;
            _session.ActiveUser.Projects = user.Projects;
            _session.ActiveUser.Certificates = user.Certificates;

            // Wype³nij kolekcje
            foreach (var w in user.WorkExperiences) WorkExperiences.Add(w);
            foreach (var e in user.Educations) Educations.Add(e);
            foreach (var p in user.Projects) Projects.Add(p);
            foreach (var c in user.Certificates) Certificates.Add(c);

            OnPropertyChanged(nameof(CurrentUser));
        }

        private async Task SavePersonalData()
        {
            await _db.UpdateUser(CurrentUser);
        }

        private async Task DeleteProfile()
        {
            await _db.DeleteUser(CurrentUser);
            _session.Logout();
        }

        private async Task AddWork()
        {
            CurrentWork.User = CurrentUser;
            await _db.AddWorkExperience(CurrentWork);
            WorkExperiences.Add(CurrentWork);
            CurrentWork = new WorkExperience { StartDate = DateTime.Today };
        }

        private async Task AddEducation()
        {
            CurrentEducation.User = CurrentUser;
            await _db.AddEducation(CurrentEducation);
            Educations.Add(CurrentEducation);
            CurrentEducation = new Education { StartDate = DateTime.Today };
        }

        private async Task AddProject()
        {
            CurrentProject.User = CurrentUser;
            await _db.AddProject(CurrentProject);
            Projects.Add(CurrentProject);
            CurrentProject = new Project();
        }

        private async Task AddCertificate()
        {
            CurrentCertificate.User = CurrentUser;
            await _db.AddCertificate(CurrentCertificate);
            Certificates.Add(CurrentCertificate);
            CurrentCertificate = new Certificate { IssueDate = DateTime.Today };
        }

        private async Task DeleteWork(WorkExperience item)
        {
            await _db.DeleteWorkExperience(item);
            WorkExperiences.Remove(item);
        }

        private async Task DeleteEducation(Education item)
        {
            await _db.DeleteEducation(item);
            Educations.Remove(item);
        }

        private async Task DeleteProject(Project item)
        {
            await _db.DeleteProject(item);
            Projects.Remove(item);
        }

        private async Task DeleteCertificate(Certificate item)
        {
            await _db.DeleteCertificate(item);
            Certificates.Remove(item);
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
