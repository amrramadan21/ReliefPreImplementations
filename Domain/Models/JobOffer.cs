using DomainLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class JobOffer
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public DateTime WorkDate { get; set; }
        public int NumberOfHours { get; set; }
        public decimal HourlyRate { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Guid CareHomeId { get; set; }
        public Gender? RequiredGender { get; set; } // Optional
    }
}
