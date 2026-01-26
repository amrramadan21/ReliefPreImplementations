using DomainLayer.Models;
using ServiceAbstractionsLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Implementations
{
    public class AdminService : IAdminService
    {
        public async Task VerifyPSWAsync(Guid pswId)
        {
            // Approve PSW Profile
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<JobRequest>> FilterQualifiedAsync(Guid offerId)
        {
            return await Task.FromResult(new List<JobRequest>());
        }
    }
}
