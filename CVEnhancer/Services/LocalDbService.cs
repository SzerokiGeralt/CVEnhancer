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

        public async Task<List<User>> GetUsersWithPictures()
        {
            // Dołącz ProfilePicture do zapytania
            List<User> users = await Db.Users
                .Include(u => u.ProfilePicture)
                .ToListAsync();
            return users;
        }

        public async Task<User> GetUserById(int Id) {
            return await Db.Users.Where(x => x.UserId == Id).FirstAsync();
        }
    }
}
