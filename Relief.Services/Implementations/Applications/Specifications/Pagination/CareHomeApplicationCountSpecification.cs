using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications.Pagination
{
    public class CareHomeApplicationCountSpecification : BaseSpecifications<JopRequest, Guid>
    {
        public CareHomeApplicationCountSpecification(Guid careHomeId)
        : base(r =>
            (r.JobOffer.CareHomeId == careHomeId || r.JobOffer.IndividualId == careHomeId)
            &&
            (r.Status == RequestStatus.QualifiedByAdmin ||
             r.Status == RequestStatus.Accepted ||
             r.Status == RequestStatus.RejectedByCareHome))
        {
        }
    }
}
