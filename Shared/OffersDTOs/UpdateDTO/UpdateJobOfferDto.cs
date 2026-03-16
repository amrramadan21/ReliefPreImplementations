using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.UpdateDTO
{
    public class UpdateJobOfferDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        public List<string>? Preferences { get; set; } = [];
        public string? Position { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Province { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public decimal? HourlyRate { get; set; }

        public List<UpdateOfferShiftDto>? Shifts { get; set; }
    }
}
