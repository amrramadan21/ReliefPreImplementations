using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Relief.ServiceAbstraction.Interfaces.Profiles;
using Shared.ProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presentation.Controllers
{
    [ApiController]
    [Route("api/profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _profileService.GetMyProfileAsync();

            return Ok(result);
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            await _profileService.UpdateProfileAsync(dto);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfile(Guid id)
        {
            var result = await _profileService.GetUserProfileAsync(id);

            return Ok(result);
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            await _profileService.UploadProfilePhotoAsync(file);

            return Ok(new { message = "Profile photo uploaded successfully" });
        }

        [HttpDelete("remove-photo")]
        public async Task<IActionResult> RemoveProfilePhoto()
        {
            await _profileService.RemoveProfilePhotoAsync();

            return Ok(new { message = "Profile photo removed successfully" });
        }
    }

}
