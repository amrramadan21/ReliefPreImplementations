using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Offers.Specifications
{
    public class OffersByOwnerSpecification
    : BaseSpecifications<JobOffer, Guid>
    {
        public OffersByOwnerSpecification(Guid ownerId)
            : base(o => o.CareHomeId == ownerId || o.IndividualId == ownerId)
        {
        }
    }
}
