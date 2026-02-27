using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.IdentityDTOs
{
    public class CompletePswProfileDto
    {
        [Required]
        public string ProofIdentityType { get; set; } = null!;

        [Required]
        public bool WorkStatus { get; set; }

        // Files
        [Required]
        public IFormFile ProofIdentityFile { get; set; } = null!;

        [Required]
        public IFormFile PswCertificateFile { get; set; } = null!;

        [Required]
        public IFormFile CVFile { get; set; } = null!;

        [Required]
        public IFormFile ImmunizationRecordFile { get; set; } = null!;

        [Required]
        public IFormFile CriminalRecordFile { get; set; } = null!;

        public IFormFile? FirstAidOrCPRFile { get; set; }
    }
}
