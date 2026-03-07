using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Applications;
using Shared.ApplyDTOs;
using Shared.QueryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presentation.Controllers
{
    [Authorize(Roles = "PSW")]
    [ApiController]
    [Route("api/applications/apply")]
    public class ApplyController : ControllerBase
    {
        private readonly IApplyService _applyService;
        private readonly IApplicationManagementService _managementService;

        public ApplyController(IApplyService applyService, IApplicationManagementService managementService)
        {
            _applyService = applyService;
            _managementService = managementService;
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
        // APPLY FOR OFFER
        // =====================================================
        [HttpPost]
        public async Task<IActionResult> Apply([FromBody] ApplyToOfferDto dto)
        {
            if (dto == null)
                return BadRequest("Application data is required.");

            await _applyService.ApplyAsync(CurrentUserId, dto);

            return Ok(new
            {
                success = true,
                message = "Application submitted successfully."
            });
        }

 
    }
}
