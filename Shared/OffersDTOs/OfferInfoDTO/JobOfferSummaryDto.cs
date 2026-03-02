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

        public string Title { get; set; } = null!;
        public string Address { get; set; } = null!;

        public decimal HourlyRate { get; set; }

    }
}
