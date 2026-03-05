using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Common
{
    public static class SortingHelper
    {
        public static bool IsDescending(string sort)
        {
            return sort.StartsWith("-");
        }

        public static string GetProperty(string sort)
        {
            return sort.StartsWith("-")
                ? sort.Substring(1)
                : sort;
        }
    }
}
