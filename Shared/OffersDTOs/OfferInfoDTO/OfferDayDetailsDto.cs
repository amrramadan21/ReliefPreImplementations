using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.OfferInfoDTO
{
    public class OfferDayDetailsDto
    {
        public Guid DayId { get; set; }
        public DateOnly Date { get; set; }

        public List<ShiftDetailsDto> Shifts { get; set; } = new();
    }
}
