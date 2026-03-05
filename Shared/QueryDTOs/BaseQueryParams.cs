using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.QueryDTOs
{
    public class BaseQueryParams
    {
        const int MaxPageSize = 50;

        public int PageIndex { get; set; } = 1;

        private int pageSize = 10;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        // مثال:
        // createdAt
        // -createdAt
        public string? Sort { get; set; }

        // search=text
        public string? Search { get; set; }
    }
}
