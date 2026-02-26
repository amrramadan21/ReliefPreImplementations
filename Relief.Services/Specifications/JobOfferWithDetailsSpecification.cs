using Relief.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Specifications
{
    public class JobOfferWithDetailsSpecification : BaseSpecifications<JobOffer, Guid>
    {
        public JobOfferWithDetailsSpecification(Guid id) : base(j => j.Id == id)
        {
            AddInclude("Days.Shifts");
        }
    }
}
