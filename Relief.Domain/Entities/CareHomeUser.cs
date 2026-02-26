using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities
{
    public class CareHomeUser : ApplicationUser
    {
        public string BusinessLicense { get; set; } = null!;
        public string LegalName { get; set; } = null!;
        public string VaccinationPolicy { get; set; } = null!;
    }
}
