using Shared.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces.Users
{
    public interface IPswService
    {
        Task CompleteProfileAsync(Guid userId, CompletePswProfileDto dto);
    }
}
