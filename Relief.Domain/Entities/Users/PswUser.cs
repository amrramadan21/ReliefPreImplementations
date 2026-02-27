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
        public Guid Id { get; set; }
        public string PrrofIdentity { get; set; } = null!;
        public bool WorkStatus { get; set; }

        public string PswCertificate { get; set; } = null!;

        public string CV { get; set; } = null!;

        public string ImmunizationRecord { get; set; } = null!;

        public string CriminalRecord { get; set; } = null!;

        public string? CPRCard { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;


    }
}
