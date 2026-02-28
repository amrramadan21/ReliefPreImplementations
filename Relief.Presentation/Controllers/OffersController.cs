using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces.Offers;
using Shared.OffersDTOs.CreateDTO;
using Shared.OffersDTOs.UpdateDTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        public async Task<IActionResult> CreateOffer([FromBody] CreateJobOfferDto dto)
        {
            var userId = User.FindFirstValue("userId");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "UserId claim not found in token" });

            if (!Guid.TryParse(userId, out var careHomeId))
                return Unauthorized(new { message = "Invalid UserId format in token" });

            var offerId = await _offerService.CreateOfferAsync(careHomeId, dto);

            return Ok(new { offerId });
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferById(Guid id)
        {
            var offer = await _offerService.GetOfferByIdAsync(id);

            if (offer == null)
                return NotFound(new { message = "Offer not found" });

            return Ok(offer);
        }

        [HttpGet]
        [Authorize] // أي حد مسجل يقدر يشوف
        public async Task<IActionResult> GetAllOffers(int pageNumber = 1,int pageSize = 5)
        {
            
            var offers = await _offerService.GetAllOffersAsync();
            return Ok(offers);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CareHome")]
        public async Task<IActionResult> UpdateOffer(Guid id, [FromBody] UpdateJobOfferDto dto)
        {
            var userId = User.FindFirstValue("userId");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var careHomeId = Guid.Parse(userId);

            var result = await _offerService.UpdateOfferAsync(id, careHomeId, dto);

            if (!result)
                return NotFound(new { message = "Offer not found" });

            return NoContent();
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "CareHome")]
        public async Task<IActionResult> DeleteOffer(Guid id)
        {
            var userId = User.FindFirstValue("userId");

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var careHomeId = Guid.Parse(userId);

            var result = await _offerService.DeleteOfferAsync(id, careHomeId);

            if (!result)
                return NotFound(new { message = "Offer not found" });

            return NoContent(); // 204
        }


    }
}
