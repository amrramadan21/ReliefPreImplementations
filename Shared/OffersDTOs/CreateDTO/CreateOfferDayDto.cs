using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.CreateDTO
{
    public class CreateOfferDayDto
    {
        public DateOnly Date { get; set; }

        public List<CreateShiftDto> Shifts { get; set; } = new();
    }
}
