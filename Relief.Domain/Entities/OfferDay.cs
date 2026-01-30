using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities
{
    public class OfferDay
    {
        public Guid Id { get; set; }

        public DateOnly Date { get; set; }

        public Guid JobOfferId { get; set; } //FK

        public JobOffer JobOffer { get; set; } = null!; // Navigation Property 

        public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
    }
}
