using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Entities.Users
{
    public class FileMetadata
    {
        public Guid Id { get; set; }

        public string OriginalFileName { get; set; } = null!;
        public string StoredFileName { get; set; } = null!;
        public string StoredPath { get; set; } = null!;

        public string ContentType { get; set; } = null!;
        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
