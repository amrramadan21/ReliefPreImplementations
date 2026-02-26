using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.OffersDTOs.UpdateDTO
{
    public class UpdateOfferShiftDto
    {
        public Guid? ShiftId { get; set; }

        public DateOnly? Date { get; set; }

        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}
