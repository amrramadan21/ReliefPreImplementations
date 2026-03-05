using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Offers.Specifications
{
    public class JobOfferWithDetailsSpecification : BaseSpecifications<JobOffer, Guid>
    {
        public JobOfferWithDetailsSpecification(Guid id) : base(j => j.Id == id)
        {
            AddInclude(o => o.Shifts );
        }
    }
}
