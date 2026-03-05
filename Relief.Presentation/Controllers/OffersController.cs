using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Relief.Domain.Exceptions;
using Relief.ServiceAbstraction.Interfaces.Offers;
using Shared.OffersDTOs.CreateDTO;
using Shared.OffersDTOs.UpdateDTO;
using Shared.QueryDTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Relief.Web.Controllers
{
    [ApiController]
    [Route("api/offers")]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;

        public OffersController(IOfferService offerService)
        {
            _offerService = offerService;
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

        private string CurrentRole
        {
            get
            {
                return User.FindFirstValue(ClaimTypes.Role) ?? "";
            }
        }

        // =====================================================
        // CREATE OFFER
        // =====================================================
        [Authorize(Roles = "CareHome,Individual")]
        [HttpPost]
        public async Task<IActionResult> CreateOffer([FromBody] CreateJobOfferDto dto)
        {
            var offerId = await _offerService.CreateOfferAsync(CurrentUserId, dto);

            return Ok(new
            {
                success = true,
                offerId
            });
        }

        // =====================================================
        // GET OFFER BY ID
        // =====================================================
        [Authorize(Roles = "CareHome,Individual,PSW")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferById(Guid id)
        {
            if (CurrentRole == "PSW")
            {
                var result = await _offerService.GetOfferDetailsForPswAsync(id);
                return Ok(result);
            }

            var result2 = await _offerService.GetOfferByIdAsync(id, CurrentUserId);
            return Ok(result2);
        }

        // =====================================================
        // GET ALL OFFERS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetAllOffers(
            [FromQuery] Guid? careHomeId = null)
        {
            var offers = await _offerService.GetAllOffersAsync(careHomeId);

            return Ok(offers);
        }

      

        // =====================================================
        // UPDATE OFFER
        // =====================================================
        [Authorize(Roles = "CareHome,Individual")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOffer(
            Guid id,
            [FromBody] UpdateJobOfferDto dto)
        {
            var result = await _offerService.UpdateOfferAsync(id, CurrentUserId, dto);

            if (!result)
                return NotFound(new { message = "Offer not found" });

            return NoContent();
        }

        // =====================================================
        // DELETE OFFER
        // =====================================================
        [Authorize(Roles = "CareHome,Individual")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffer(Guid id)
        {
            var result = await _offerService.DeleteOfferAsync(id, CurrentUserId);

            if (!result)
                return NotFound(new { message = "Offer not found" });

            return NoContent();
        }
    }
}
