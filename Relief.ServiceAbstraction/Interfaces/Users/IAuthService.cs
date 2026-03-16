using Shared.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces.Users
{
    public interface IAuthService
    {
        Task<RegisterResponseDTO> RegisterUserAsync(RegisterDTO dto, string role);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
        Task<AuthResponseDTO> VerifyEmailAsync(VerifyEmailDTO dto);
        Task<RegisterResponseDTO> ResendVerificationCodeAsync(ResendCodeDTO dto);
        Task LogoutAsync();
    }
}
