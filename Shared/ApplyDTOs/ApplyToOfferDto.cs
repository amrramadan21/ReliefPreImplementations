using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ApplyDTOs
{
    public class ApplyToOfferDto
    {
        [Required]
        public Guid OfferId { get; set; }

        public List<Guid> ShiftIds { get; set; } = new();
    }
}
