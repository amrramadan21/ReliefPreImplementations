using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ApplyDTOs
{
    public class ApplyToOfferDto
    {
        public Guid OfferId { get; set; }

        public List<Guid> ShiftIds { get; set; } = new();
    }
}
