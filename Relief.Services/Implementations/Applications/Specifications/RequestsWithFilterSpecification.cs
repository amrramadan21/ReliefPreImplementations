using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using Relief.Services.Common;

namespace Relief.Services.Specifications
{
    public class RequestsWithFilterSpecification
        : BaseSpecifications<JopRequest, Guid>
    {
        public RequestsWithFilterSpecification(
            Guid careHomeId,
            RequestStatus? status,
            int pageIndex,
            int pageSize)
        : base(r =>
            r.JobOffer.CareHomeId == careHomeId)
        {
            AddInclude(r => r.Items);
            AddInclude(r => r.PswUser);

            AddOrderByDescending(r => r.CreatedAt);

            ApplyPagination(pageIndex, pageSize);
        }
    }
}