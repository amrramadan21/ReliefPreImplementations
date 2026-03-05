using Relief.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.QueryDTOs
{
    public class RequestQueryParams : BaseQueryParams
    {
        public RequestStatus? Status { get; set; }

    }
}
