using Relief.Domain.Entities.Users;
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
    public class PswUserPaginatedSpecification : BaseSpecifications<PswUser, Guid>
    {
        public PswUserPaginatedSpecification(AdminPswQueryParams query)
        : base(BuildCriteria(query))
        {
            AddInclude(p => p.ApplicationUser);

           

            ApplyPagination(query.PageIndex, query.PageSize);
        }

        private static Expression<Func<PswUser, bool>> BuildCriteria(AdminPswQueryParams query)
        {
            if (query.VerificationStatus.HasValue)
            {
                return p => p.VerificationStatus == query.VerificationStatus.Value;
            }

            return p => true;  // no filter — return all
        }
    }
}
