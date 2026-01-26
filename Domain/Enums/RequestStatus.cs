using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Enums
{
    public enum RequestStatus
    {
        Pending=1,
        QualifiedByAdmin=2,
        AcceptedByCareHome=3,
        Rejected=4    
    }
}
