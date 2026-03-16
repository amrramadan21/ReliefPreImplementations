using Microsoft.AspNetCore.Http;
using Shared.ProfileDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces.Profiles
{
    public interface IProfileService
    {
        Task<object> GetMyProfileAsync();

        Task<object> GetUserProfileAsync(Guid userId);

        Task UpdateProfileAsync(UpdateProfileDto dto);
        Task UploadProfilePhotoAsync(IFormFile file);
        Task RemoveProfilePhotoAsync();
    }
}
