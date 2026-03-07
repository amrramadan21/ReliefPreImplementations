using Relief.Domain.Enums;

namespace Shared.AdminDTOs
{
    public class PswVerificationListDto
    {
        public Guid PswUserId { get; set; }

        public string FullName { get; set; } = "";

        public string Email { get; set; } = "";

        public string? ProofIdentityType { get; set; }

        public VerificationStatus VerificationStatus { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime ProfileCompletedAt { get; set; }

        public Guid? ProofIdentityFileId { get; set; }

        public Guid? PswCertificateFileId { get; set; }

        public Guid? CVFileId { get; set; }

        public Guid? ImmunizationRecordFileId { get; set; }

        public Guid? CriminalRecordFileId { get; set; }

        public Guid? FirstAidOrCPRFileId { get; set; }
    }
}
