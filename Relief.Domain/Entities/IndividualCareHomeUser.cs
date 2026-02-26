using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities
{
    public class IndividualCareHomeUser : ApplicationUser
    {
        ICollection<JobOffer> JobOffers { get; set; } = new List<JobOffer>();

    }
}
