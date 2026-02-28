using Relief.Domain.Entities.Users;
using Relief.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities
{
    public class JopRequest
    {
        public Guid Id { get; set; }

        public Guid PswId { get; set; }
        public PswUser PswUser { get; set; } = null!;   // 👈 Navigation

        public Guid JobOfferId { get; set; }

        public RequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<JobRequestItem> Items { get; set; } = new List<JobRequestItem>();
    }
}
