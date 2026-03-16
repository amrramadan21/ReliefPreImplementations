using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Admin.Specifications.Pagination
{
    public class AdminApplicationCountSpecification : BaseSpecifications<JopRequest, Guid>
    {
        public AdminApplicationCountSpecification(RequestStatus? status)
        : base(status.HasValue
            ? (Expression<Func<JopRequest, bool>>)(r => r.Status == status.Value)
            : null!)
        {
        }
    }
}
