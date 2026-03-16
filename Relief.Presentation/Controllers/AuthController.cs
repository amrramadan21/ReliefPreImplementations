using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces.Users;
using Shared.IdentityDTOs;

namespace Relief.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        // =====================================================
        // REGISTER CARE HOME
        // =====================================================
        [HttpPost("register/carehome")]
        public async Task<ActionResult<RegisterResponseDTO>> RegisterCareHome(
            [FromBody] RegisterDTO dto)
        {
            if (dto == null)
                return BadRequest("Registration data is required.");

            var result = await _auth.RegisterUserAsync(dto, "CareHome");

            return Ok(result);
        }

        // =====================================================
        // REGISTER INDIVIDUAL
        // =====================================================
        [HttpPost("register/individual")]
        public async Task<ActionResult<RegisterResponseDTO>> RegisterIndividual(
            [FromBody] RegisterDTO dto)
        {
            if (dto == null)
                return BadRequest("Registration data is required.");

            var result = await _auth.RegisterUserAsync(dto, "Individual");

            return Ok(result);
        }

        // =====================================================
        // REGISTER PSW
        // =====================================================
        [HttpPost("register/psw")]
        public async Task<ActionResult<RegisterResponseDTO>> RegisterPsw(
            [FromBody] RegisterDTO dto)
        {
            if (dto == null)
                return BadRequest("Registration data is required.");

            var result = await _auth.RegisterUserAsync(dto, "PSW");

            return Ok(result);
        }

        // =====================================================
        // ✅ NEW: VERIFY EMAIL
        // =====================================================
        [HttpPost("verify-email")]
        public async Task<ActionResult<AuthResponseDTO>> VerifyEmail(
            [FromBody] VerifyEmailDTO dto)
        {
            var result = await _auth.VerifyEmailAsync(dto);
            return Ok(result);
        }

        // =====================================================
        // ✅ NEW: RESEND VERIFICATION CODE
        // =====================================================
        [HttpPost("resend-verification")]
        public async Task<ActionResult<RegisterResponseDTO>> ResendVerification(
            [FromBody] ResendCodeDTO dto)
        {
            var result = await _auth.ResendVerificationCodeAsync(dto);
            return Ok(result);
        }

        // =====================================================
        // LOGIN
        // =====================================================
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login(
            [FromBody] LoginDTO dto)
        {
            if (dto == null)
                return BadRequest("Login data is required.");

            var result = await _auth.LoginAsync(dto);

            return Ok(result);
        }

        // =====================================================
        // LOGOUT
        // =====================================================
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _auth.LogoutAsync();

            return Ok(new
            {
                message = "Logged out successfully"
            });
        }
    }
}
