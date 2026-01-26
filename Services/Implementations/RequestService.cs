using DomainLayer.Models;
using ServiceAbstractionsLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Implementations
{
    public class RequestService : IRequestService
    {
        public async Task<JobRequest> ApplyAsync(Guid offerId, Guid pswId)
        {
            var request = new JobRequest
            {
                Id = Guid.NewGuid(),
                JobOfferId = offerId,
                PSWId = pswId
            };

            return await Task.FromResult(request);
        }

        public async Task<IEnumerable<JobRequest>> GetRequestsForOfferAsync(Guid offerId)
        {
            return await Task.FromResult(new List<JobRequest>());
        }
    }
}
