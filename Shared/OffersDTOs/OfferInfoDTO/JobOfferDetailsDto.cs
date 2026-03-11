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
        public Guid? CareHomeId { get; set; }
        public string? CareHomeName { get; set; }
        public Guid? IndividualId { get; set; }
        public string? IndividualName { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public string Address { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public decimal HourlyRate { get; set; }

        public List<OfferShiftDetailsDto> Shifts { get; set; } = new();
    }
}
