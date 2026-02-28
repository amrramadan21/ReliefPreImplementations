using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications
{
    public class OfferApplicationsSpecification
     : BaseSpecifications<JopRequest, Guid>
    {
        public OfferApplicationsSpecification(Guid offerId)
            : base(r => r.JobOfferId == offerId)
        {
            AddInclude(r => r.PswUser);
            AddInclude("PswUser.ApplicationUser");
            AddInclude(r => r.Items);
            AddInclude("Items.OfferShift");
        }
    }
}
