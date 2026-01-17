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
        
        public event EventHandler<bool>? AuthenticationChanged;

        public bool IsAuthenticated => ActiveUser != null;

        public void Login(User user) {
            ActiveUser = user;
            AuthenticationChanged?.Invoke(this, true);
        }

        public void Logout() {
            ActiveUser = null;
            AuthenticationChanged?.Invoke(this, false);
        }
    }
}
