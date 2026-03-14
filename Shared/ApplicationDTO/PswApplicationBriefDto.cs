using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ApplicationDTO
{
    public class PswApplicationBriefDto
    {
        public Guid PswId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public int Age { get; set; }
        public string VerificationStatus { get; set; } = default!;
        public string? RejectionReason { get; set; }
        public string? ProofIdentityType { get; set; }
        public Guid? CVFileId { get; set; }
    }
}
