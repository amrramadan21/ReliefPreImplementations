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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public RequestStatus Status { get; set; }

        // psw relation
        public Guid PswId  { get; set; }
        public PswUser PswUser { get; set; } = null!;

        public ICollection<OfferShift> OfferShift { get; set; } = new List<OfferShift>();

    }
}
