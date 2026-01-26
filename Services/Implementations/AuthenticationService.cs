using DomainLayer.Models;
using ServiceAbstractionsLayer.Interfaces;

namespace ServicesLayer.Implementations
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;

        public AuthenticationService(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            user.Id = await _identityService.CreateUserAsync(user, password);
            return user;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _identityService.GetByEmailAsync(email);
            if (user == null)
                throw new Exception("Invalid credentials");

            var isValid = await _identityService.CheckPasswordAsync(email, password);
            if (!isValid)
                throw new Exception("Invalid credentials");

            return await _identityService.GenerateJwtAsync(user);
        }
    }
}
