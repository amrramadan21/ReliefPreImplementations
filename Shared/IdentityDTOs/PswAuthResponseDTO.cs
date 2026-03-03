using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.IdentityDTOs
{
    public class PswAuthResponseDTO : AuthResponseDTO
    {
        public bool WorkStatus { get; set; } = default!;
    }
}
