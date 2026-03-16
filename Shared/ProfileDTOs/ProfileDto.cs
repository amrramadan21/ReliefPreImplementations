using Shared.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProfileDTOs
{
    public class ProfileDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; } = default!;

        public AddressDTO Address { get; set; } = default!;
        public FileDto? ProfilePhoto { get; set; }

    }
}
