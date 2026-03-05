using Microsoft.AspNetCore.Identity;
using Relief.Domain.Enums;

namespace Relief.Domain.Entities.Users
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Insurance { get; set; }

        public Gender Gender { get; set; }

        public Guid AddressId { get; set; }
        public Address? Address { get; set; }

        public DateTime BirthOfDate { get; set; }

        // Profiles (Optional)
        public CareHomeUser? CareHomeUser { get; set; }

        public IndividualCareHomeUser? IndividualCareHomeUser { get; set; }

        public PswUser? PswUser { get; set; }
    }
}