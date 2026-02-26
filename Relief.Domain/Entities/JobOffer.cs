using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities
{
    public class JobOffer
    { 

    
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public decimal HourlyRate { get; set; }


        //Location
        public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Guid CareHomeId { get; set; }
        public ICollection<OfferShift> Shifts { get; set; } = new List<OfferShift>();




    }
}
