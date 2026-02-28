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
        public bool WorkStatus { get; set; }

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
        public bool IsVerified { get; set; } = false;

        public ICollection<JopRequest> JobRequests { get; set; } = new List<JopRequest>();
    }
}
