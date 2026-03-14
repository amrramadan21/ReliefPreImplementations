using Relief.Domain.Entities.Offers;
using Relief.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities.Users
{
    public class PswUser
    {
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; } = null!;

        public string? ProofIdentityType { get; set; }
        public Guid? ProofIdentityFileId { get; set; }
        public FileMetadata? ProofIdentityFile { get; set; }

        public Guid? InsuranceFileId { get; set; }
        public FileMetadata? InsuranceFile { get; set; }

        public Guid? PswCertificateFileId { get; set; }
        public FileMetadata? PswCertificateFile { get; set; }

        public Guid? CVFileId { get; set; }
        public FileMetadata? CVFile { get; set; }

        public Guid? ImmunizationRecordFileId { get; set; }
        public FileMetadata? ImmunizationRecordFile { get; set; }

        public Guid? CriminalRecordFileId { get; set; }
        public FileMetadata? CriminalRecordFile { get; set; }

        public Guid? FirstAidOrCPRFileId { get; set; }
        public FileMetadata? FirstAidOrCPRFile { get; set; }

        public bool IsProfileCompleted { get; set; } = false;

        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.None;

        public string? VerificationRejectionReason { get; set; }

        public ICollection<JopRequest> JobRequests { get; set; } = new List<JopRequest>();
    }
}