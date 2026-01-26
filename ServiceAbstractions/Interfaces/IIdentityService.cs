using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstractionsLayer.Interfaces
{
    public interface IIdentityService
    {
        Task<Guid> CreateUserAsync(User user, string password);
        Task<bool> CheckPasswordAsync(string email, string password);
        Task<string> GenerateJwtAsync(User user);
        Task<User?> GetByEmailAsync(string email);
    }
}
