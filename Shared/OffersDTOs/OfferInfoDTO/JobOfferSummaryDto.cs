using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.OfferInfoDTO
{
    public class JobOfferSummaryDto
    {
        public Guid Id { get; set; }
        public Guid? PosterId { get; set; }
        public string? PosterName { get; set; }
        public string PosterType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Position { get; set; } = null!;
        public List<string> Preferences { get; set; } = [];
        public string Address { get; set; } = null!;
        public string? Address2 { get; set; }
        public string City { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string Province { get; set; } = null!;
        public decimal HourlyRate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }


    }
}
