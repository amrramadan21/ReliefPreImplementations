using System.ComponentModel.DataAnnotations;

namespace Shared.AdminDTOs
{
    public class AdminRejectDto
    {
        [Required]
        public string Reason { get; set; } = null!;
    }
}
