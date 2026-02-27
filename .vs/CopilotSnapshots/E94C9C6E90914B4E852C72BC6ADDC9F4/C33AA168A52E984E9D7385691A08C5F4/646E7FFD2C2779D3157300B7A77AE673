using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces;
using Shared.IdentityDTOs;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Relief.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompleteProfileController : ControllerBase
    {
        private readonly IPswService _pswService;

        public CompleteProfileController(IPswService pswService)
        {
            _pswService = pswService;
        }

        [Authorize(Roles = "PSW")]
        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromForm] CompletePswProfileDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue("userId")!);

            await _pswService.CompleteProfileAsync(userId, dto);

            return Ok(new
            {
                message = "Profile completed and verified successfully."
            });
        }
    }
}
