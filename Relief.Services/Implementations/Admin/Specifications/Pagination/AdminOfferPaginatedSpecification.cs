using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using Shared.QueryDTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Admin.Specifications.Pagination
{
    public class AdminOfferPaginatedSpecification : BaseSpecifications<JobOffer, Guid>
    {
        public AdminOfferPaginatedSpecification(AdminOfferQueryParams query)
        : base()
        {
            AddInclude(o => o.Shifts);

            AddInclude(o => o.CareHomeUser);
            AddInclude("CareHomeUser.ApplicationUser");

            AddInclude(o => o.IndividualCareHomeUser);
            AddInclude("IndividualCareHomeUser.ApplicationUser");

            ApplyPagination(query.PageIndex, query.PageSize);
        }
    }
}
