using CVEnhancer.Data;
using CVEnhancer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVEnhancer.Services
{
    public class LocalDbService
    {
        public AppDbContext Db { get; private set; } = new AppDbContext();

        public LocalDbService()
        {
            // Tworzenie bazy jeśli nie istnieje
            Db.Database.EnsureCreated();
        }

        // ===== USER =====
        public async Task<List<User>> GetUsersWithPictures()
        {
            // Dołącz ProfilePicture do zapytania
            List<User> users = await Db.Users
                .Include(u => u.ProfilePicture)
                .ToListAsync();
            return users;
        }

        public async Task<User> GetUserById(int id)
        {
            return await Db.Users.Where(x => x.UserId == id).FirstAsync();
        }

        public async Task<User?> GetUserWithAllData(int userId)
        {
            return await Db.Users
                .Where(u => u.UserId == userId)
                .AsSplitQuery()
                .Include(u => u.WorkExperiences)
                    .ThenInclude(w => w.Skills)
                .Include(u => u.Projects)
                    .ThenInclude(p => p.Skills)
                .Include(u => u.Educations)
                    .ThenInclude(p => p.Skills)
                .Include(u => u.Certificates)
                    .ThenInclude(p => p.Skills)
                .FirstOrDefaultAsync();
        }
        /// <summary>
        /// Zwraca bez skilli
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User?> GetUserWithAllDataMinimal(int userId)
        {
            return await Db.Users
                .Where(u => u.UserId == userId)
                .AsSplitQuery()
                .Include(u => u.WorkExperiences)
                .Include(u => u.Projects)
                .Include(u => u.Educations)
                .Include(u => u.Certificates)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateUser(User user)
        {
            Db.Users.Update(user);
            await Db.SaveChangesAsync();
        }

        public async Task AddUser(User user)
        {
            Db.Users.Add(user);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteUser(User user)
        {
            Db.Users.Remove(user);
            await Db.SaveChangesAsync();
        }

        public async Task<List<Skill>> GetAllSkillsAsync()
        {
            return await Db.Skills.ToListAsync();
        }

        public async Task<Dictionary<string, string>> GetSkillAliasMapAsync()
        {
            var skills = await Db.Skills.ToListAsync();
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var skill in skills)
            {
                // Dodaj samą nazwę skilla (mapuje na siebie)
                map.TryAdd(skill.Name, skill.Name);

                // Dodaj wszystkie aliasy mapujące na kanoniczną nazwę
                if (skill.Aliases != null)
                {
                    foreach (var alias in skill.Aliases)
                    {
                        if (!string.IsNullOrWhiteSpace(alias))
                        {
                            map.TryAdd(alias, skill.Name);
                        }
                    }
                }
            }

            return map;
        }

        // ===== WORK EXPERIENCE =====
        public async Task AddWorkExperience(WorkExperience item)
        {
            Db.WorkExperiences.Add(item);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteWorkExperience(WorkExperience item)
        {
            Db.WorkExperiences.Remove(item);
            await Db.SaveChangesAsync();
        }

        // ===== EDUCATION =====
        public async Task AddEducation(Education item)
        {
            Db.Educations.Add(item);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteEducation(Education item)
        {
            Db.Educations.Remove(item);
            await Db.SaveChangesAsync();
        }

        // ===== PROJECTS =====
        public async Task AddProject(Project item)
        {
            Db.Projects.Add(item);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteProject(Project item)
        {
            Db.Projects.Remove(item);
            await Db.SaveChangesAsync();
        }

        // ===== CERTIFICATES =====
        public async Task AddCertificate(Certificate item)
        {
            Db.Certificates.Add(item);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteCertificate(Certificate item)
        {
            Db.Certificates.Remove(item);
            await Db.SaveChangesAsync();
        }

        public async Task<int> CountUsers()
        {
            return await Db.Users.CountAsync();
        }
    }
}
