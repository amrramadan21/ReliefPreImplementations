using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Applications;
using Shared.ApplicationDTO;
using Shared.QueryDTOs;
using Shared.QueryDTOs.CareHome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presentation.Controllers
{

    [Authorize(Roles = "CareHome,Individual")]
    [ApiController]
    [Route("api/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly IApplicationManagementService _applicationService;
        private readonly IApplyService _applyService;

        public ApplicationsController(
            IApplicationManagementService applicationService,
            IApplyService applyService)
        {
            _applicationService = applicationService;
            _applyService = applyService;
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
                    throw new UnauthorizedException("Invalid token.");

                return Guid.Parse(id);
            }
        }

        // =====================================================
        // GET ALL APPLICATIONS FOR ALL OFFERS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetAllApplications([FromQuery] CareHomeApplicationQueryParams query)
        {
            var result = await _applicationService
                .GetApplicationsForCareHomeAsync(CurrentUserId, query);

            return Ok(result);
        }

        // =====================================================
        // GET APPLICATIONS FOR SPECIFIC OFFER
        // =====================================================
        [HttpGet("{offerId}")]
        public async Task<IActionResult> GetApplications(Guid offerId)
        {
            var result = await _applicationService
                .GetApplicationsForOfferAsync(offerId, CurrentUserId);

            return Ok(result);
        }

        // =====================================================
        // ACCEPT SHIFT
        // =====================================================
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptShift(
            [FromBody] AcceptShiftDto dto)
        {
            await _applicationService.AcceptShiftAsync(
                dto.ShiftId,
                dto.JobRequestItemId,
                CurrentUserId);

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
            await _applicationService.RejectShiftAsync(
                dto.JobRequestItemId,
                CurrentUserId);

            return Ok(new
            {
                success = true,
                message = "Application rejected successfully."
            });
        }

        
    }
}
