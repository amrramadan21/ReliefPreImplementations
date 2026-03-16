using Relief.Domain.Entities.Users;
using Relief.Services.Common;
using Shared.QueryDTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Relief.Services.Implementations.Admin.Specifications.Pagination
{
    public class PswUserCountSpecification : BaseSpecifications<PswUser, Guid>
    {
        public PswUserCountSpecification(AdminPswQueryParams query)
            : base(BuildCriteria(query))
        {
           
        }

        private static Expression<Func<PswUser, bool>> BuildCriteria(AdminPswQueryParams query)
        {
            if (query.VerificationStatus.HasValue)
            {
                return p => p.VerificationStatus == query.VerificationStatus.Value;
            }

            return p => true;
        }
    }
}
