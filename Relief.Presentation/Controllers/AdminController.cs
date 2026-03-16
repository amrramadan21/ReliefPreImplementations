using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces.Admin;
using Relief.ServiceAbstraction.Interfaces.Profiles;
using Shared.AdminDTOs;
using Shared.QueryDTOs.Admin;

namespace Relief.Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IProfileService _profileService;

        public AdminController(IAdminService adminService, IProfileService profileService)
        {
            _adminService = adminService;
            _profileService = profileService;
        }

        // =====================================================
        // PSW VERIFICATION — LIST PENDING
        // =====================================================
        [HttpGet("verifications/pending")]
        public async Task<IActionResult> GetPendingVerifications([FromQuery] PendingVerificationQueryParams query)
        {
            var result = await _adminService.GetPendingVerificationsAsync(query);

            return Ok(result);
        }

        // =====================================================
        // PSW VERIFICATION — APPROVE
        // =====================================================
        [HttpPost("verifications/{pswId}/approve")]
        public async Task<IActionResult> ApproveVerification(Guid pswId)
        {
            await _adminService.ApproveVerificationAsync(pswId);

            return Ok(new
            {
                success = true,
                message = "PSW verification approved."
            });
        }

        // =====================================================
        // PSW VERIFICATION — REJECT
        // =====================================================
        [HttpPost("verifications/{pswId}/reject")]
        public async Task<IActionResult> RejectVerification(
            Guid pswId,
            [FromBody] AdminRejectDto dto)
        {
            await _adminService.RejectVerificationAsync(pswId, dto.Reason);

            return Ok(new
            {
                success = true,
                message = "PSW verification rejected."
            });
        }

        // =====================================================
        // APPLICATION REVIEW — LIST BY STATUS
        // =====================================================
        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications(
        [FromQuery] AdminApplicationQueryParams query)
        {
            var result = await _adminService.GetApplicationsByStatusAsync(query);

            return Ok(result);
        }

        // =====================================================
        // APPLICATION REVIEW — APPROVE
        // =====================================================
        [HttpPost("applications/{requestId}/approve")]
        public async Task<IActionResult> ApproveApplication(Guid requestId)
        {
            await _adminService.ApproveApplicationAsync(requestId);

            return Ok(new
            {
                success = true,
                message = "Application approved and forwarded to CareHome."
            });
        }

        // =====================================================
        // APPLICATION REVIEW — REJECT
        // =====================================================
        [HttpPost("applications/{requestId}/reject")]
        public async Task<IActionResult> RejectApplication(
            Guid requestId,
            [FromBody] AdminRejectDto dto)
        {
            await _adminService.RejectApplicationAsync(requestId, dto.Reason);

            return Ok(new
            {
                success = true,
                message = "Application rejected by admin."
            });
        }

        // =====================================================
        // OFFERS — VIEW ALL
        // =====================================================
        [HttpGet("offers")]
        public async Task<IActionResult> GetAllOffers([FromQuery] AdminOfferQueryParams query)
        {
            var result = await _adminService.GetAllOffersAsync(query);

            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsersByRole([FromQuery] string? role)
        {
            var result = await _adminService.GetUsersByRoleAsync(role);
            return Ok(new
            {
                success = true,
                count = result.Count,
                data = result
            });

        }

        [HttpGet("users/PSW")]
        public async Task<IActionResult> GetAllPsw([FromQuery] AdminPswQueryParams query)
        {
            var result = await _adminService.GetAllPswUsersAsync(query);
            return Ok(result);
        }

        [HttpGet("users/profile")]
        public async Task<IActionResult> GetUserProfile(Guid id)
        {
            var result = await _profileService.GetUserProfileAsync(id);
            return Ok(new
            {
                success = true,
                data = result
            });

        }

    }
}
