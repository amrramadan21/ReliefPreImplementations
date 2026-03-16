using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Admin.Specifications.Pagination
{
    public class AdminOfferCountSpecification : BaseSpecifications<JobOffer, Guid>
    {
        public AdminOfferCountSpecification()
        : base()
        {
        }
    }
}
