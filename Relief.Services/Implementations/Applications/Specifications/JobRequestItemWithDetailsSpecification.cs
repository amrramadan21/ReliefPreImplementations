using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications
{
    public class JobRequestItemWithDetailsSpecification
     : BaseSpecifications<JobRequestItem, Guid>
    {
        public JobRequestItemWithDetailsSpecification(Guid itemId)
            : base(i => i.Id == itemId)
        {
            AddInclude(i => i.JopRequest);
            AddInclude("JopRequest.PswUser");
            AddInclude(i => i.OfferShift);
        }
    }
}
