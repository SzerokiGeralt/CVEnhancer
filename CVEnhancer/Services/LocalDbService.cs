using CVEnhancer.Data;
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

        public async Task AddUser()
        {
            Db.Add(new Models.User
            {
                FirstName = "Karol",
                LastName = "Kapusta"

            });
            await Db.SaveChangesAsync();
        }
    }
}
