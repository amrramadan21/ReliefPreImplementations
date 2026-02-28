using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications
{
    public class PswApplicationsSpecification
     : BaseSpecifications<JopRequest, Guid>
    {
        public PswApplicationsSpecification(Guid pswId)
            : base(r => r.PswId == pswId)
        {
            AddInclude(r => r.Items);
            AddInclude("Items.OfferShift");
            AddInclude("Items.OfferShift.JobOffer");
        }
    }
}
