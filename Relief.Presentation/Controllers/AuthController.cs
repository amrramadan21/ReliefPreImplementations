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
        public async Task<ActionResult<AuthResponseDTO>> RegisterCareHome(
            [FromBody] RegisterCareHomeDto dto)
        {
            if (dto == null)
                return BadRequest("Registration data is required.");

            var result = await _auth.RegisterCareHomeAsync(dto);

            return Ok(result);
        }

        // =====================================================
        // REGISTER INDIVIDUAL
        // =====================================================
        [HttpPost("register/individual")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterIndividual(
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
        public async Task<ActionResult<AuthResponseDTO>> RegisterPsw(
            [FromBody] RegisterDTO dto)
        {
            if (dto == null)
                return BadRequest("Registration data is required.");

            var result = await _auth.RegisterUserAsync(dto, "PSW");

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
    }
}
