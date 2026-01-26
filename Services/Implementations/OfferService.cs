using DomainLayer.Models;
using ServiceAbstractionsLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.Implementations
{
    public class OfferService : IOfferService
    {
        public async Task<JobOffer> CreateOfferAsync(JobOffer offer)
        {
            // Save Offer
            return await Task.FromResult(offer);
        }

        public async Task<IEnumerable<JobOffer>> GetAllOffersAsync()
        {
            return await Task.FromResult(new List<JobOffer>());
        }

        public async Task<JobOffer?> GetOfferByIdAsync(Guid id)
        {
            return await Task.FromResult<JobOffer?>(null);
        }
    }
}
