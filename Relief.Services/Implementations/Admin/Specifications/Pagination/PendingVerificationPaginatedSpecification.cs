using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using Relief.Services.Common;
using Shared.QueryDTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Admin.Specifications.Pagination
{
    public class PendingVerificationPaginatedSpecification : BaseSpecifications<PswUser, Guid>
    {
        public PendingVerificationPaginatedSpecification(PendingVerificationQueryParams query)
        : base(p => 
                     p.VerificationStatus == VerificationStatus.Pending)
        {
            AddInclude(p => p.ApplicationUser);

            ApplyPagination(query.PageIndex, query.PageSize);
        }
    }
}
