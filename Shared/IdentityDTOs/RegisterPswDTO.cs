using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.IdentityDTOs
{
    public class RegisterPswDTO
    {
        [Required]
        public string FirstName { get; set; } = default!;
        [Required]
        public string LastName { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = default!;
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = default!;
        [Required]
        public DateTime DateOfBirth { get; set; } = default!;
        [Required]
        public string Gender { get; set; } = default!;

        [Required]

        public string PrrofIdentity { get; set; } = null!;
        [Required]

        public bool WorkStatus { get; set; }
        [Required]

        public string PswCertificate { get; set; } = null!;
        [Required]

        public string CV { get; set; } = null!;
        [Required]

        public string ImmunizationRecord { get; set; } = null!;
        [Required]

        public string CriminalRecord { get; set; } = null!;
        [Required]

        public string? CPRCard { get; set; }
        [Required]
        public AddressDTO Address { get; set; } = null!;

    }
}
