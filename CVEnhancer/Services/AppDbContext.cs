using Microsoft.EntityFrameworkCore;
using CVEnhancer.Models;
using CVEnhancer.Utils;

namespace CVEnhancer.Data
{
    public class AppDbContext : DbContext
    {
        // DbSet = tabela w bazie
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<WorkExperience> WorkExperiences { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillAlias> SkillAliases { get; set; }
        public DbSet<SkillCategory> SkillCategories { get; set; }
        public DbSet<GeneratedCV> GeneratedCVs { get; set; }
        public DbSet<ProfilePicture> ProfilePictures { get; set; }

        // Ścieżka do pliku bazy
        public string DbPath { get; }

        public AppDbContext()
        {
            try
            {
                // MAUI - folder LocalApplicationData (runtime)
                var path = FileSystem.AppDataDirectory;
                DbPath = Path.Combine(path, "CVELite.db");
            }
            catch
            {
                // Design-time - użyj temp path dla migracji
                DbPath = Path.Combine(Path.GetTempPath(), "CVELite.db");
            }
        }

        // Konfiguracja połączenia
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlite($"Data Source={DbPath}");
                options.UseSeeding((context, _) =>
                {
                    // Sprawdź czy użytkownicy już istnieją
                    var existingUser = context.Set<User>().FirstOrDefault();
                    if (existingUser == null)
                    {
                        // Dodaj przykładowych użytkowników
                        context.Set<User>().AddRange(
                            new User
                            {
                                FirstName = "Jan",
                                LastName = "Kowalski",
                                ProfilePicture = new ProfilePicture() {Picture = ImageByteConverter.defaultImage },
                                Email = "jan.kowalski@example.com",
                                PhoneNumber = "+48 123 456 789",
                                LinkedInUrl = "https://linkedin.com/in/jankowalski",
                                GitHubUrl = "https://github.com/jankowalski",
                                JobTitle = "Senior .NET Developer",
                                ProfessionalSummary = "Doświadczony programista .NET z 5-letnim stażem w tworzeniu aplikacji enterprise."
                            },
                            new User
                            {
                                FirstName = "Anna",
                                LastName = "Nowak",
                                ProfilePicture = new ProfilePicture() { Picture = ImageByteConverter.defaultImage },
                                Email = "anna.nowak@example.com",
                                PhoneNumber = "+48 987 654 321",
                                LinkedInUrl = "https://linkedin.com/in/annanowak",
                                GitHubUrl = "https://github.com/annanowak",
                                JobTitle = "Full Stack Developer",
                                ProfessionalSummary = "Wszechstronna programistka z pasją do tworzenia nowoczesnych aplikacji webowych i mobilnych."
                            },
                            new User
                            {
                                FirstName = "Piotr",
                                LastName = "Wiśniewski",
                                ProfilePicture = new ProfilePicture() { Picture = ImageByteConverter.defaultImage },
                                Email = "piotr.wisniewski@example.com",
                                PhoneNumber = "+48 555 123 456",
                                LinkedInUrl = "https://linkedin.com/in/piotrwisniewski",
                                PortfolioUrl = "https://piotrwisniewski.dev",
                                JobTitle = "Junior Software Developer",
                                ProfessionalSummary = "Młody programista pełen entuzjazmu, specjalizujący się w technologiach Microsoft."
                            }
                        );
                        context.SaveChanges();
                    }
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfiguracja relacji User -> ProfilePicture z cascade delete
            modelBuilder.Entity<User>()
                .HasOne(u => u.ProfilePicture)
                .WithOne(p => p.User)
                .HasForeignKey<ProfilePicture>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Konfiguracja cascade delete dla pozostałych relacji
            modelBuilder.Entity<User>()
                .HasMany(u => u.WorkExperiences)
                .WithOne(w => w.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Educations)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Projects)
                .WithOne(p => p.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Certificates)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
