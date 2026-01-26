using DomainLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class JobRequest
    {
        public Guid Id { get; set; }

        public Guid JobOfferId { get; set; }
        public Guid PSWId { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
