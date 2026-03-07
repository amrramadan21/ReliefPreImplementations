using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.CreateDTO
{
    public class CreateJobOfferDto
    {
        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public decimal HourlyRate { get; set; }

        public List<CreateOfferShiftDto> Shifts { get; set; } = new();
    }
}
