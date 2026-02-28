using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ApplicationDTO
{
    public class AcceptShiftDto
    {
        public Guid ShiftId { get; set; }
        public Guid JobRequestItemId { get; set; }
    }
}
