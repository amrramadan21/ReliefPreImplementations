using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications
{
    public class ShiftApplicationsSpecification
    : BaseSpecifications<JobRequestItem, Guid>
    {
        public ShiftApplicationsSpecification(Guid shiftId)
            : base(i => i.ShiftId == shiftId)
        {
            AddInclude(i => i.JopRequest);
        }
    }
}
