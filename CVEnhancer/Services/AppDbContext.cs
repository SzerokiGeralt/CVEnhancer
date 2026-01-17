using Microsoft.EntityFrameworkCore;
using CVEnhancer.Models;

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
            }
        }
    }
}
