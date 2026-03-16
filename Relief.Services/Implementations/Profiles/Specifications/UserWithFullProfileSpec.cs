using Relief.Domain.Entities.Users;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Profiles.Specifications
{
    public class UserWithFullProfileSpec : BaseSpecifications<ApplicationUser, Guid>
    {
        public UserWithFullProfileSpec(Guid userId)
        : base(u => u.Id == userId)
        {
            // ── Common includes ──
            AddInclude(u => u.Address!);
            AddInclude(u => u.ProfilePhoto!);

            // ── PSW includes (string-based for nested navigation) ──
            AddInclude("PswUser");
            AddInclude("PswUser.ProofIdentityFile");
            AddInclude("PswUser.InsuranceFile");
            AddInclude("PswUser.PswCertificateFile");
            AddInclude("PswUser.CVFile");
            AddInclude("PswUser.ImmunizationRecordFile");
            AddInclude("PswUser.CriminalRecordFile");
            AddInclude("PswUser.FirstAidOrCPRFile");

            ApplySplitQuery();
        }
    }
}
