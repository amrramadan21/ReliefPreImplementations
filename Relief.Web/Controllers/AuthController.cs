using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces;
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

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var result = await _auth.RegisterAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var result = await _auth.LoginAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
