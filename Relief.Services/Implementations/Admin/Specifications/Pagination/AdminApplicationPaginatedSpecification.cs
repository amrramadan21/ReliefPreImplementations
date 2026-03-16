using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using Relief.Services.Common;
using Shared.QueryDTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Admin.Specifications.Pagination
{
    public class AdminApplicationPaginatedSpecification : BaseSpecifications<JopRequest, Guid>
    {
        public AdminApplicationPaginatedSpecification(
        AdminApplicationQueryParams query, RequestStatus? status)
        : base(status.HasValue
           ? (Expression<Func<JopRequest, bool>>)(r => r.Status == status.Value)
           : null!)
        {
            AddInclude(r => r.JobOffer);
            AddInclude("PswUser.ApplicationUser");
            AddInclude("Items.OfferShift");

            AddOrderByDescending(r => r.CreatedAt);
            ApplySplitQuery();  // This fixes the Cartesian product

            ApplyPagination(query.PageIndex, query.PageSize);
        }
    }
}
