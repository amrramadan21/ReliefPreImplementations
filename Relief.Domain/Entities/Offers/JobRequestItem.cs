using Relief.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities.Offers
{
    public class JobRequestItem
    {
        public Guid Id { get; set; }
        public Guid JopRequestId { get; set; }

        // Navigation property to parent JopRequest
        public JopRequest JopRequest { get; set; } = null!;
       
        public Guid ShiftId { get; set; }

        // Navigation property to OfferShift
        public OfferShift OfferShift { get; set; } = null!;

        public RequestStatus Status { get; set; }
    }
}
