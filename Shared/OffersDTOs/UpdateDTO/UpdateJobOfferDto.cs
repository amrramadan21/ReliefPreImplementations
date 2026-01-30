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

        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public decimal? HourlyRate { get; set; }

        public List<UpdateOfferDayDto>? Days { get; set; }
    }
}
