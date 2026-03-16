using Microsoft.AspNetCore.Http;
using Shared.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProfileDTOs
{
    public class UpdateProfileDto
    {
        // ── Common (all roles) ──
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
        public AddressDTO? Address { get; set; }
        public IFormFile? ProfilePhoto { get; set; }

        // ── PSW-only ──
        public string? ProofIdentityType { get; set; }
        public IFormFile? ProofIdentityFile { get; set; }
        public IFormFile? InsuranceFile { get; set; }
        public IFormFile? PswCertificateFile { get; set; }
        public IFormFile? CVFile { get; set; }
        public IFormFile? ImmunizationRecordFile { get; set; }
        public IFormFile? CriminalRecordFile { get; set; }
        public IFormFile? FirstAidOrCPRFile { get; set; }
    }
}
