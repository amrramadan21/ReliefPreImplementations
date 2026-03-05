using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces.Applications;
using Shared.ApplicationDTO;
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
    [Route("api/psw/applications")]
    public class PswApplicationsController : ControllerBase
    {
        private readonly IApplicationManagementService _service;

        public PswApplicationsController(
            IApplicationManagementService service)
        {
            _service = service;
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
        // GET MY APPLICATIONS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetMyApplications()
        {
            var result = await _service.GetPswApplicationsAsync(CurrentUserId);

            return Ok(result);
        }

        // =====================================================
        // CANCEL APPLICATION
        // =====================================================
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelApplication(
            [FromBody] CancelApplicationDto dto)
        {
            if (dto == null)
                return BadRequest("Application data is required.");

            await _service.CancelApplicationAsync(
                dto.JobRequestItemId,
                CurrentUserId);

            return Ok(new
            {
                success = true,
                message = "Application cancelled successfully."
            });
        }
    }
}
