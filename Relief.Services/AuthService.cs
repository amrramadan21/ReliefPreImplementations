using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Relief.Domain.Entities;
using Relief.ServiceAbstraction;
using Shared.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto)
        {
            if (dto.Role != "CareHome" && dto.Role != "PSW")
                throw new InvalidOperationException("Role must be 'CareHome' or 'PSW'.");

            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("Email already registered.");

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                BirthOfDate = dto.DateOfBirth
            };

            var create = await _userManager.CreateAsync(user, dto.Password);
            if (!create.Succeeded)
                throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));

            var addRole = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!addRole.Succeeded)
                throw new InvalidOperationException(string.Join("; ", addRole.Errors.Select(e => e.Description)));

            return await BuildTokenAsync(user);
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                       ?? throw new InvalidOperationException("Invalid credentials.");

            var ok = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!ok) throw new InvalidOperationException("Invalid credentials.");

            return await BuildTokenAsync(user);
        }

        private async Task<AuthResponseDTO> BuildTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "";

            var jwt = _config.GetSection("Jwt");
            var key = jwt["Key"]!;
            var issuer = jwt["Issuer"]!;
            var audience = jwt["Audience"]!;
            var expiryMinutes = int.Parse(jwt["ExpiryMinutes"] ?? "60");

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? "")
            };

            if (!string.IsNullOrWhiteSpace(role))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAtUtc = expires,
                Email = user.Email ?? "",
                Role = role,
                UserId = user.Id
            };
        }
    }
}
