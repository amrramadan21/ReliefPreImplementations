using Relief.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Specifications
{
    public class JobOfferShiftSpecification : BaseSpecifications<OfferShift, Guid>
    {
        public JobOfferShiftSpecification(Guid careHomeId, DateOnly? date) 
            : base(shift => shift.JobOffer.CareHomeId == careHomeId && shift.Date == date)
        {
        }
    }
}
