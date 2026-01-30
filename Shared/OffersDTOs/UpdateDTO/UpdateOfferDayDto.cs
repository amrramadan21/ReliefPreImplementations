using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.UpdateDTO
{
    public class UpdateOfferDayDto
    {
        public Guid? DayId { get; set; }

        public DateOnly Date { get; set; }

        public List<UpdateShiftDto> Shifts { get; set; } = new();
    }
}
