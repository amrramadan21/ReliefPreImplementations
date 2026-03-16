using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.QueryDTOs.Admin
{
    public class AdminApplicationQueryParams : BaseQueryParams
    {
        public string? Status { get; set; }
    }
}
