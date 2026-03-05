using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;

namespace Relief.Domain.Entities.Offers
{
    public class JopRequest
    {
        public Guid Id { get; set; }

        public Guid PswId { get; set; }

        public PswUser PswUser { get; set; } = null!;

        public Guid JobOfferId { get; set; }

        public JobOffer JobOffer { get; set; } = null!;

        public RequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<JobRequestItem> Items { get; set; } = new List<JobRequestItem>();
    }
}