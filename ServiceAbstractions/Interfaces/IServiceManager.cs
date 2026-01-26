using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstractionsLayer.Interfaces
{
    public interface IServiceManager
    {
        IAuthenticationService AuthenticationService { get; }
        IOfferService OfferService { get; }
        IRequestService RequestService { get; }
        IAdminService AdminService { get; }
    }
}
