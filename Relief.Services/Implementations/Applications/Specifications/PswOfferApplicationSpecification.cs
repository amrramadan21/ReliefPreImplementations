using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using Relief.Services.Common;

public class PswOfferApplicationSpecification
    : BaseSpecifications<JopRequest, Guid>
{
    public PswOfferApplicationSpecification(Guid pswId, Guid offerId)
        : base(r => r.PswId == pswId &&
                    r.JobOfferId == offerId &&
                    r.Status == RequestStatus.Pending)
    {
    }
}