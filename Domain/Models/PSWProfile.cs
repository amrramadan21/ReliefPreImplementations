using DomainLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class PSWProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public int Age { get; set; }
        public Gender Gender { get; set; }

        public string Brief { get; set; } = null!;
        public decimal HourlyRate { get; set; }

        // Confidential (Admin Only)
        public string CertificatePath { get; set; } = null!;
        public string WorkPermitPath { get; set; } = null!;
        public string NationalIdPath { get; set; } = null!;

        public bool IsApprovedByAdmin { get; set; }

    }
}
