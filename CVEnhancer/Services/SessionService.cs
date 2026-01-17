using CVEnhancer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVEnhancer.Services
{
    public class SessionService
    {
        public User? ActiveUser { get; private set; }

        public void Login(User user) {
            ActiveUser = user;
        }

        public void Logout() {
            ActiveUser = null;
        }
    }
}
