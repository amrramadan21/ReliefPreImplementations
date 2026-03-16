using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.QueryDTOs
{
    public class BaseQueryParams
    {
        private const int MaxPageSize = 20;
        private const int DefaultPageSize = 10;

        public int PageIndex { get; set; } = 1;

        private int pageSize = DefaultPageSize;

        public int PageSize
        {
            get => pageSize;
            set
            {
                if (value <= 0)
                {
                    pageSize = DefaultPageSize;
                }
                else if (value > MaxPageSize)
                {
                    pageSize = MaxPageSize;
                }
                else
                    pageSize = value;
            }
        }

        // مثال:
        // createdAt
        // -createdAt
        public string? Sort { get; set; }

        // search=text
        public string? Search { get; set; }
    }
}
