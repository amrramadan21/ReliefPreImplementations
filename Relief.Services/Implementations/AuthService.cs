using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Relief.Domain.Entities;
using Relief.Domain.Enums;
using Relief.ServiceAbstraction.Interfaces;
using Shared.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations
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

        public async Task<AuthResponseDTO> RegisterCareHomeAsync(RegisterCareHomeDTO dto)
        {
            var user = new CareHomeUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                BirthOfDate = dto.DateOfBirth,
                Gender = Enum.Parse<Gender>(dto.Gender, true),



                // Map any CareHome-specific fields here
            };

            // Hardcode the role here based on the endpoint called
            return await RegisterUserCoreAsync(user, dto.Password, "CareHome");
        }

        public async Task<AuthResponseDTO> RegisterIndividualAsync(RegisterIndividualDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                BirthOfDate = dto.DateOfBirth
                // Map any Individual-specific fields here
            };

            // Hardcode the role here
            return await RegisterUserCoreAsync(user, dto.Password, "Individual");
        }

        public async Task<AuthResponseDTO> RegisterPswAsync(RegisterPswDTO dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                BirthOfDate = dto.DateOfBirth
                // Map any PSW-specific fields here
            };

            // Hardcode the role here
            return await RegisterUserCoreAsync(user, dto.Password, "PSW");
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
    new Claim("userId", user.Id.ToString()),
    new Claim(ClaimTypes.Role, role ?? ""),
    new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
};

            Console.WriteLine("TOKEN KEY USED: " + key);



            //if (!string.IsNullOrWhiteSpace(role))
            //    claims.Add(new Claim(ClaimTypes.Role, role));

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


        private async Task<AuthResponseDTO> RegisterUserCoreAsync(ApplicationUser user, string password, string role)
        {
            // 1. Check if user exists
            var existing = await _userManager.FindByEmailAsync(user.Email??"Wrone Email");
            if (existing != null)
                throw new InvalidOperationException("Email already registered.");

            // 2. Create the user
            var create = await _userManager.CreateAsync(user, password);
            if (!create.Succeeded)
                throw new InvalidOperationException(string.Join("; ", create.Errors.Select(e => e.Description)));

            // 3. Assign the hardcoded role
            var addRole = await _userManager.AddToRoleAsync(user, role);
            if (!addRole.Succeeded)
                throw new InvalidOperationException(string.Join("; ", addRole.Errors.Select(e => e.Description)));

            // 4. Return token
            return await BuildTokenAsync(user);
        }
    }
}
