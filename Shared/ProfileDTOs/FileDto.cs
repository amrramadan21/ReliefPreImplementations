using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ProfileDTOs
{
    public class FileDto
    {
        public Guid Id { get; set; }

        public string FileName { get; set; } = null!;

        public string Url { get; set; } = null!;
        public string ContentType { get; set; } = null!;
    }
}
