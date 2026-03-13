using Relief.Domain.Entities.Users;
using Relief.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Implementations.Admin.Specifications
{
    public class PswUserWithDetailsSpecification : BaseSpecifications<PswUser, Guid>
    {
        public PswUserWithDetailsSpecification() : base()
        {
            // Eagerly load the ApplicationUser
            AddInclude(p => p.ApplicationUser!);

            // If you also want to load the Address from the ApplicationUser, 
            // you can use the string-based include from your base class:
            // AddInclude("ApplicationUser.Address");
        }
    }
}
