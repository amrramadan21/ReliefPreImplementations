using Relief.Domain.Entities.Offers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities.Users
{
    public class CareHomeUser
    {
        public Guid Id { get; set; }
        public string BusinessLicense { get; set; } = null!;
        public string LegalName { get; set; } = null!;
        public string? VaccinationPolicy { get; set; }

        // no need the id will be the same as the application user id and it will be the primary key as well
        //public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public ICollection<JobOffer> JobOffers { get; set; } = new List<JobOffer>();

    }
}
