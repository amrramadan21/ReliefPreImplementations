using DomainLayer.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistenceLayer.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public RoleType Role { get; set; }
        public bool IsVerifiedByAdmin { get; set; }
    }
}
