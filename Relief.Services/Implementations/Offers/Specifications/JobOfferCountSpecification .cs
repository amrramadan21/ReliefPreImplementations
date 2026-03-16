using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using Shared.QueryDTOs.JobOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Offers.Specifications
{
    public class JobOfferCountSpecification : BaseSpecifications<JobOffer, Guid>
    {
        public JobOfferCountSpecification(JobOfferQueryParams query)
        : base(BuildCriteria(query))
        {
        }

        private static Expression<Func<JobOffer, bool>> BuildCriteria(JobOfferQueryParams query)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (query.CareHomeId.HasValue)
            {
                return o =>
                    (o.CareHomeId == query.CareHomeId.Value ||
                     o.IndividualId == query.CareHomeId.Value)
                    &&
                    o.Shifts.Any(s => s.IsAvailable && s.Date >= today);
            }

            return o => o.Shifts.Any(s => s.IsAvailable && s.Date >= today);
        }

    }
}
