using System.ComponentModel.DataAnnotations;

namespace Shared.IdentityDTOs
{
    public class RegisterCareHomeDto
    {
        [Required]
        public string LegalName { get; set; } = default!;

        [Required]
        public string BusinessLicense { get; set; } = default!;

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
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = default!;

        [Required]
        public AddressDTO Address { get; set; } = default!;
    }
}