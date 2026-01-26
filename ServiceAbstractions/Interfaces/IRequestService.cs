using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstractionsLayer.Interfaces
{
    public interface IRequestService
    {
        Task<JobRequest> ApplyAsync(Guid offerId, Guid pswId);
        Task<IEnumerable<JobRequest>> GetRequestsForOfferAsync(Guid offerId);
    }
}
