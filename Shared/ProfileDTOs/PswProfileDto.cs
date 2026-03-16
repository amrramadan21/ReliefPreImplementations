using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProfileDTOs
{
    public class PswProfileDto : ProfileDto
    {
        public string? ProofIdentityType { get; set; }

        public string VerificationStatus { get; set; } = default!;

        public string? RejectionReason { get; set; }




        public FileDto? ProofIdentityFile { get; set; }

        public FileDto? InsuranceFile { get; set; }

        public FileDto? PswCertificateFile { get; set; }

        public FileDto? CVFile { get; set; }

        public FileDto? ImmunizationRecordFile { get; set; }

        public FileDto? CriminalRecordFile { get; set; }

        public FileDto? FirstAidOrCPRFile { get; set; }
    }
}
