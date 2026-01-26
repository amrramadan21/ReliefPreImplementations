using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstractionsLayer.Interfaces
{
    public interface IAdminService
    {
        Task VerifyPSWAsync(Guid pswId);
        Task<IEnumerable<JobRequest>> FilterQualifiedAsync(Guid offerId);
    }
}
