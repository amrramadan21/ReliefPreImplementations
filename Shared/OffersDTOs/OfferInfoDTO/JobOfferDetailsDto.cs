using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.OfferInfoDTO
{
    public class JobOfferDetailsDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public decimal HourlyRate { get; set; }

        public List<OfferShiftDetailsDto> Shifts { get; set; } = new();
    }
}
