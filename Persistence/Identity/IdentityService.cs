using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersistenceLayer.Identity;
using ServiceAbstractionsLayer.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersistenceLayer.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public IdentityService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<Guid> CreateUserAsync(User user, string password)
        {
            var appUser = new ApplicationUser
            {
                UserName = user.Email,
                Email = user.Email,
                Role = user.Role,
                IsVerifiedByAdmin = user.IsVerifiedByAdmin
            };

            var result = await _userManager.CreateAsync(appUser, password);
            if (!result.Succeeded)
                throw new Exception("Registration failed");

            return user.Id;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return null;

            return new User
            {
                Id = Guid.Parse(appUser.Id),
                Email = appUser.Email!,
                Role = appUser.Role,
                IsVerifiedByAdmin = appUser.IsVerifiedByAdmin
            };
        }

        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            var appUser = await _userManager.FindByEmailAsync(email);
            if (appUser == null) return false;

            return await _userManager.CheckPasswordAsync(appUser, password);
        }

        public async Task<string> GenerateJwtAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    double.Parse(_configuration["JwtSettings:DurationInMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
