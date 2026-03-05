using Relief.Domain.Entities.Users;

namespace Relief.Domain.Entities.Offers
{
    public class OfferShift
    {
        public Guid Id { get; set; }

        public DateOnly? Date { get; set; }

        public Guid JobOfferId { get; set; }

        public JobOffer JobOffer { get; set; } = null!;

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public bool IsAvailable { get; set; } = true;

        public Guid? AssignedPswId { get; set; }

        public PswUser? AssignedPsw { get; set; }
    }
}