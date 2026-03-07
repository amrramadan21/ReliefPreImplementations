using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Relief.Domain.Entities.Offers
{
    public class JopRequest
    {
        public Guid Id { get; set; }

        public Guid PswId { get; set; }

        public PswUser PswUser { get; set; } = null!;

        public Guid JobOfferId { get; set; }

        public JobOffer JobOffer { get; set; } = null!;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public string? RejectionReason { get; set; }

        public ICollection<JobRequestItem> Items { get; set; } = new List<JobRequestItem>();
    }
}