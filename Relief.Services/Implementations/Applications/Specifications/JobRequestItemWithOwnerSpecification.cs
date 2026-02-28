using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications
{
    public class JobRequestItemWithOwnerSpecification
      : BaseSpecifications<JobRequestItem, Guid>
    {
        public JobRequestItemWithOwnerSpecification(Guid itemId)
            : base(i => i.Id == itemId)
        {
            AddInclude(i => i.JopRequest);
            AddInclude(i => i.OfferShift);
        }
    }
}
