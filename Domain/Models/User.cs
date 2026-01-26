using DomainLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class User
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public RoleType Role { get; set; }

        // CareHome = Verified automatically
        // PSW = Needs Admin Verification
        public bool IsVerifiedByAdmin { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsPSW => Role == RoleType.PersonalSupprotWorker;
        public bool IsCareHome => Role == RoleType.CareHome;
    }
}
