using Microsoft.AspNetCore.Identity;
using Relief.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        // need more info to decide if this is necessary
        public string? Insurance { get; set; }
        public Gender Gender { get; set; }
        public Guid AddressId { get; set; }
        public Address? Address { get; set; }
        public DateTime BirthOfDate { get; set; } = default!;
    }
}
