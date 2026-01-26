using DomainLayer.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersistenceLayer.Identity;
using ServiceAbstractionsLayer.Interfaces;
using Shared.DTOs.Auth;
using System.Runtime.InteropServices;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new DomainLayer.Models.User
            {
                FullName = dto.Email,
                Email = dto.Email,
                Role = dto.Role == "CareHome" ? RoleType.CareHome : RoleType.PersonalSupprotWorker,
                IsVerifiedByAdmin = dto.Role == "CareHome"
            };

            await _authService.RegisterAsync(user, dto.Password);
            return Ok("Registered Successfully");
        }

       
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto.Email, dto.Password);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

    }
}
