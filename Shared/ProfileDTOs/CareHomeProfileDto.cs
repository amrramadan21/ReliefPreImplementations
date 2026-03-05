using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProfileDTOs
{
    public class CareHomeProfileDto : ProfileDto
    {
        public string BusinessLicense { get; set; }

        public string LegalName { get; set; }

        public string VaccinationPolicy { get; set; }
    }
}
