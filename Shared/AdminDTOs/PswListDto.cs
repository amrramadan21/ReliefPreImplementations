using Relief.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.AdminDTOs
{
    public class PswListDto : UserListDto
    {
        public VerificationStatus VerificationStatus { get; set; }
        public string? VerificationRejectionReason { get; set; }
        public bool IsProfileCompleted { get; set; } 
        public bool IsVerified { get; set; }
    }
}
