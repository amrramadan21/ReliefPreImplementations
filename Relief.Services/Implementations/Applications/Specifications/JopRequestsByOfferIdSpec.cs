using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications
{
    public class JopRequestsByOfferIdSpec :
        BaseSpecifications<JopRequest, Guid>
    {
        public JopRequestsByOfferIdSpec(Guid offerId)
            : base(r => r.JobOfferId == offerId)
        {
        }
    }
}
