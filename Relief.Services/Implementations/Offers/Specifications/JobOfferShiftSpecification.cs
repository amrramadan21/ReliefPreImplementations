using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Offers.Specifications
{
    public class JobOfferShiftSpecification : BaseSpecifications<OfferShift, Guid>
    {
        public JobOfferShiftSpecification(Guid careHomeId, DateOnly? date) 
            : base(shift => shift.JobOffer.CareHomeId == careHomeId && shift.Date == date)
        {
        }
    }
}
