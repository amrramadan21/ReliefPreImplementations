using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using Relief.Services.Common;

public class PswOfferApplicationSpecification
    : BaseSpecifications<JopRequest, Guid>
{
    public PswOfferApplicationSpecification(Guid pswId, List<Guid> shiftIds)
        : base(r => r.PswId == pswId &&
                    r.Items.Any(i => shiftIds.Contains(i.ShiftId)))
    {
    }
}