using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Admin.Specifications.Pagination
{
    public class PendingVerificationCountSpecification : BaseSpecifications<PswUser, Guid>
    {
        public PendingVerificationCountSpecification()
        : base(p => p.VerificationStatus == VerificationStatus.Pending)
        {
        }
    }
}
