using Relief.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ApplicationDTO
{
    public class ShiftApplicationDto
    {
        public Guid JobRequestItemId { get; set; }   
        public Guid ShiftId { get; set; }
        public DateOnly? Date { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public RequestStatus Status { get; set; }
    }
}
