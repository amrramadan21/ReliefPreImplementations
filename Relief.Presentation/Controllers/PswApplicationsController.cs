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

        [HttpGet]
        public async Task<IActionResult> GetMyApplications()
        {
            var pswId = Guid.Parse(User.FindFirstValue("userId")!);

            var result = await _service.GetPswApplicationsAsync(pswId);

            return Ok(result);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelApplication(
                                        [FromBody] CancelApplicationDto dto)
        {
            var pswId = Guid.Parse(User.FindFirstValue("userId")!);

            await _service.CancelApplicationAsync(
                dto.JobRequestItemId,
                pswId);

            return Ok(new
            {
                success = true,
                message = "Application cancelled successfully."
            });
        }
    }
}
