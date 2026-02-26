using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.CreateDTO
{
    public class CreateJobOfferDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public decimal HourlyRate { get; set; }

        public List<CreateOfferShiftDto> Shifts { get; set; } = new();
    }
}
