using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ApplicationDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Relief.ServiceAbstraction.Interfaces.Applications;

namespace Relief.Presentation.Controllers
{

    [Authorize(Roles = "CareHome")]
    [ApiController]
    [Route("api/carehome/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationManagementService _applicationService;

        public ApplicationsController(
            IApplicationManagementService applicationService)
        {
            _applicationService = applicationService;
        }

        // =====================================================
        // GET ALL APPLICATIONS FOR OFFER
        // =====================================================
        [HttpGet("{offerId}")]
        public async Task<IActionResult> GetApplications(Guid offerId)
        {
            var careHomeId = Guid.Parse(
                User.FindFirstValue("userId")!);

            var result = await _applicationService
                .GetApplicationsForOfferAsync(offerId, careHomeId);

            return Ok(result);
        }

        // =====================================================
        // ACCEPT SHIFT
        // =====================================================
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptShift(
            [FromBody] AcceptShiftDto dto)
        {
            var careHomeId = Guid.Parse(
                User.FindFirstValue("userId")!);

            await _applicationService.AcceptShiftAsync(
                dto.ShiftId,
                dto.JobRequestItemId,
                careHomeId);

            return Ok(new
            {
                success = true,
                message = "Shift accepted successfully."
            });
        }

        // =====================================================
        // REJECT SHIFT
        // =====================================================
        [HttpPost("reject")]
        public async Task<IActionResult> RejectShift(
            [FromBody] RejectShiftDto dto)
        {
            var careHomeId = Guid.Parse(
                User.FindFirstValue("userId")!);

            await _applicationService.RejectShiftAsync(
                dto.JobRequestItemId,
                careHomeId);

            return Ok(new
            {
                success = true,
                message = "Application rejected successfully."
            });
        }
    }
}
