using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications
{
    public class CareHomeApplicationsSpecification
     : BaseSpecifications<JopRequest, Guid>
    {
        public CareHomeApplicationsSpecification(List<Guid> offerIds)
            : base(r => offerIds.Contains(r.JobOfferId))
        {
            AddInclude(r => r.PswUser);
            AddInclude("PswUser.ApplicationUser");

            AddInclude(r => r.Items);
            AddInclude("Items.OfferShift");
        }
    }
}
