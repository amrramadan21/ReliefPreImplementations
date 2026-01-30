using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces;
using Shared.OffersDTOs.CreateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
        }

        // 🔐 CareHome only
        [Authorize(Roles = "CareHome")]
        [HttpPost]
        public async Task<IActionResult> CreateOffer(CreateJobOfferDto dto)
        {
            var careHomeId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var offerId = await _offerService.CreateOfferAsync(careHomeId, dto);

            return CreatedAtAction(
                nameof(GetOfferById),
                new { id = offerId },
                new { offerId }
            );
        }

        [HttpGet("{id}")]
        public IActionResult GetOfferById(Guid id)
        {
            return Ok();
        }
    }

}
