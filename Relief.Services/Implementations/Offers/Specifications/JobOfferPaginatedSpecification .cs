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
    public class JobOfferPaginatedSpecification : BaseSpecifications<JobOffer, Guid>
    {
        public JobOfferPaginatedSpecification(JobOfferQueryParams query)
            : base(BuildCriteria(query))
        {
            // Include CareHomeUser for LegalName
            AddInclude(o => o.CareHomeUser);
            AddInclude("CareHomeUser.ApplicationUser");

            // Include IndividualCareHomeUser -> ApplicationUser for FirstName/LastName
            AddInclude(o => o.IndividualCareHomeUser);
            AddInclude("IndividualCareHomeUser.ApplicationUser");


            AddInclude(o => o.Shifts);

            ApplyPagination(query.PageIndex, query.PageSize);
        }
        private static Expression<Func<JobOffer, bool>> BuildCriteria(JobOfferQueryParams query)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // ✅ Must have at least one available & not expired shift
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
