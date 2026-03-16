using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presistence
{
    public class S3Settings
    {
        public string BucketName { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
    }
}
