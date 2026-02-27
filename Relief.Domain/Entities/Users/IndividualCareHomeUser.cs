using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities.Users
{
    public class IndividualCareHomeUser
    {
        public Guid Id { get; set; }
        
        
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public ICollection<JobOffer> JobOffers { get; set; } = new List<JobOffer>();

    }
}
