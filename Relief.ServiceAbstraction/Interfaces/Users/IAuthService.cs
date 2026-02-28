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
        Task<AuthResponseDTO> RegisterUserAsync(RegisterDTO dto, string role);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
    }
}
