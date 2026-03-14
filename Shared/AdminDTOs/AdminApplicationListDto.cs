using Relief.Domain.Enums;
using Shared.ApplicationDTO;

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
        public string PswPhone { get; set; } = "";
        public string PswEmail { get; set; } = "";
        public string VerificationStatus { get; set; } = default!;
        public string? VerificationReason { get; set; }
        public List<ShiftApplicationDto> Shifts { get; set; } = new ();
    }
}
