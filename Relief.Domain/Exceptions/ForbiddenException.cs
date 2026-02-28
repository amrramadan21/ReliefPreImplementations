using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message)
            : base(message, (int)HttpStatusCode.Forbidden)
        {
        }
    }
}
