using Relief.Domain.Entities.Offers;
using Relief.Services.Common;
using Shared.QueryDTOs.Psw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Applications.Specifications.Pagination
{
    public class PswApplicationPaginatedSpecification : BaseSpecifications<JobRequestItem, Guid>
    {
        public PswApplicationPaginatedSpecification(Guid pswId, PswApplicationQueryParams query)
        : base(item => item.JopRequest.PswId == pswId)
        {
            AddInclude(item => item.JopRequest);
            AddInclude("OfferShift.JobOffer");

            ApplyPagination(query.PageIndex, query.PageSize);
        }

    }
}
