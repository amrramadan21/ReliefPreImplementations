using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Exceptions
{
    public class ValidationException : AppException
    {
        public IEnumerable<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base("Validation failed", (int)HttpStatusCode.UnprocessableEntity)
        {
            Errors = errors;
        }
    }
}
