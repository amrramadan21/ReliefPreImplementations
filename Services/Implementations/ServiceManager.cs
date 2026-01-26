using ServiceAbstractionsLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Implementations
{
    public class ServiceManager :IServiceManager
    {
        public IAuthenticationService AuthenticationService { get; }
        public IOfferService OfferService { get; }
        public IRequestService RequestService { get; }
        public IAdminService AdminService { get; }

        public ServiceManager(
            IAuthenticationService authenticationService,
            IOfferService offerService,
            IRequestService requestService,
            IAdminService adminService)
        {
            AuthenticationService = authenticationService;
            OfferService = offerService;
            RequestService = requestService;
            AdminService = adminService;
        }
    }
}
