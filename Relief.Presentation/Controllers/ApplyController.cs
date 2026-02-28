using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.ApplyDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Relief.ServiceAbstraction.Interfaces.Applications;

namespace Relief.Presentation.Controllers
{
    [Authorize(Roles = "PSW")]
    [ApiController]
    [Route("api/apply")]
    public class ApplyController : ControllerBase
    {
        private readonly IApplyService _applyService;

        public ApplyController(IApplyService applyService)
        {
            _applyService = applyService;
        }

        [HttpPost]
        public async Task<IActionResult> Apply([FromBody] ApplyToOfferDto dto)
        {
            var pswId = Guid.Parse(User.FindFirstValue("userId")!);

            await _applyService.ApplyAsync(pswId, dto);

            return Ok("Application submitted successfully.");
        }
    }
}
