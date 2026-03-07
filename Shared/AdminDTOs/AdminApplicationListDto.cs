using Relief.Domain.Enums;

namespace Shared.AdminDTOs
{
    public class AdminApplicationListDto
    {
        public Guid JobRequestId { get; set; }

        public Guid OfferId { get; set; }

        public string OfferTitle { get; set; } = "";

        public RequestStatus Status { get; set; }

        public DateTime AppliedAt { get; set; }

        public string? RejectionReason { get; set; }

        // PSW Info
        public Guid PswId { get; set; }

        public string PswFullName { get; set; } = "";

        public bool IsVerified { get; set; }

        // Shift count
        public int ShiftCount { get; set; }
    }
}
