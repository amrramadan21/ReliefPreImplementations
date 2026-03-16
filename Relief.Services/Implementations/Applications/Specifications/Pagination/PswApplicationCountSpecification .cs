using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications.Pagination
{
    public class PswApplicationCountSpecification : BaseSpecifications<JobRequestItem, Guid>
    {
        public PswApplicationCountSpecification(Guid pswId)
        : base(item => item.JopRequest.PswId == pswId)
        {
        }
    }
}
