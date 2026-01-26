using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstractionsLayer.Interfaces
{
    public interface IAuthenticationService
    {
            Task<User> RegisterAsync(User user, string password);
            Task<string> LoginAsync(string email, string password);
  
    }
}
