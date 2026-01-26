using ServiceAbstractionsLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Implementations
{
    public static class ApplicationServicesRegistration 
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<IRequestService, RequestService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IServiceManager, ServiceManager>();

            return services;
        }
    }
}
