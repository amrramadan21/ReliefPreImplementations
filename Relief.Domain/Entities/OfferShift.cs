using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities
{
    public class OfferShift
    {
        public Guid Id { get; set; }

        public DateOnly? Date { get; set; }

        public Guid JobOfferId { get; set; } //FK
        public JobOffer JobOffer { get; set; } = null!; // Navigation Property 

        // Optional relation to JobRequest
        public Guid? JobRequestId { get; set; }
        public JopRequest? JopRequest { get; set; }


        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
        public Guid? AssignedPswId { get; set; }
    }
}
