using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ApplicationDTO
{
    public class OfferApplicationDto
    {
        public Guid JobRequestId { get; set; }
        public DateTime AppliedAt { get; set; }
        public PswApplicationBriefDto Psw { get; set; } = null!;
        public List<ShiftApplicationDto> Shifts { get; set; } = new();
    }
}
