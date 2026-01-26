using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstractionsLayer.Interfaces
{
    public interface IOfferService
    {
        Task<JobOffer> CreateOfferAsync(JobOffer offer);
        Task<IEnumerable<JobOffer>> GetAllOffersAsync();
        Task<JobOffer?> GetOfferByIdAsync(Guid id);
    }
}
