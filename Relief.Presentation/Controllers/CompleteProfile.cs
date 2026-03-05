using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces.Users;
using Shared.IdentityDTOs;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Relief.Presentation.Controllers
{
    [Authorize(Roles = "PSW")]
    [ApiController]
    [Route("api/psw/profile")]
    public class CompleteProfileController : ControllerBase
    {
        private readonly IPswService _pswService;

        public CompleteProfileController(IPswService pswService)
        {
            _pswService = pswService;
        }

        // =====================================================
        // Helper
        // =====================================================
        private Guid CurrentUserId
        {
            get
            {
                var id = User.FindFirstValue("userId");

                if (id == null)
                    throw new UnauthorizedAccessException("Invalid token.");

                return Guid.Parse(id);
            }
        }

        // =====================================================
        // COMPLETE PSW PROFILE
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> CompleteProfile(
            [FromForm] CompletePswProfileDto dto)
        {
            if (dto == null)
                return BadRequest("Profile data is required.");

            await _pswService.CompleteProfileAsync(CurrentUserId, dto);

            return Ok(new
            {
                success = true,
                message = "Profile completed and verified successfully."
            });
        }
    }
}
