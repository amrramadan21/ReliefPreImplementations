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

        [HttpPost("register/carehome")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterCareHome([FromBody] RegisterCareHomeDTO dto)
        {
            try
            {
                var result = await _auth.RegisterCareHomeAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register/individual")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterIndividual([FromBody] RegisterIndividualDTO dto)
        {
            try
            {
                var result = await _auth.RegisterIndividualAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register/psw")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterPsw([FromBody] RegisterPswDTO dto)
        {
            try
            {
                var result = await _auth.RegisterPswAsync(dto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
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
