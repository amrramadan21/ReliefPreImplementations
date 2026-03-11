using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Admin.Specifications
{
    public class PendingJopRequestsWithDetailsSpecification : BaseSpecifications<JopRequest, Guid>
    {
        public PendingJopRequestsWithDetailsSpecification(RequestStatus? status)
        // 1. Filter directly in the database
        : base(req => !status.HasValue || req.Status == status.Value)
        {
            // 2. Include the parent Job Offer
            AddInclude(req => req.JobOffer);

            // 3. Include the bridge table (Items) AND the actual Shift details
            // We use the string include here because it's a nested (ThenInclude) relationship
            AddInclude("Items.OfferShift");
        }
    }
}
