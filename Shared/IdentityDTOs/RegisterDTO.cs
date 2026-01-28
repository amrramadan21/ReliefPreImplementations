using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.IdentityDTOs
{
    public class RegisterDTO
    {
        [Required]
        public string FullName { get; set; } = default!;
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
        public string Role { get; set; } = default!; // "CareHome" or "PSW"
    }
}
