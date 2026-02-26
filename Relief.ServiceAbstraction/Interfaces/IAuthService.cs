using Shared.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterCareHomeAsync(RegisterCareHomeDTO dto);
        Task<AuthResponseDTO> RegisterIndividualAsync(RegisterIndividualDTO dto);
        Task<AuthResponseDTO> RegisterPswAsync(RegisterPswDTO dto);
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);
    }
}
