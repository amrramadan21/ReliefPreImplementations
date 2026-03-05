namespace Relief.Domain.Entities.Users
{
    public class Address
    {
        public Guid Id { get; set; }

        public int ApartmentNumber { get; set; }

        public string Street { get; set; } = null!;

        public string City { get; set; } = null!;

        public string State { get; set; } = null!;

        public string PostalCode { get; set; } = null!;

        public string Country { get; set; } = null!;

        public Guid UserId { get; set; }

        public ApplicationUser User { get; set; } = null!;
    }
}