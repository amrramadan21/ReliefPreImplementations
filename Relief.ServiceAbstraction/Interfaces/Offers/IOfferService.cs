using Relief.Domain.Entities;
using Relief.Domain.Entities.Offers;
using Shared.OffersDTOs.CreateDTO;
using Shared.OffersDTOs.OfferInfoDTO;
using Shared.OffersDTOs.UpdateDTO;
using Shared.QueryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces.Offers
{
    public interface IOfferService
    {
        Task<Guid> CreateOfferAsync(Guid careHomeId, CreateJobOfferDto dto);
        Task<JobOfferDetailsDto?> GetOfferByIdAsync(Guid id, Guid careHomeId);
        Task<List<JobOfferSummaryDto>> GetAllOffersAsync(Guid? careHomeId);
        Task<bool> UpdateOfferAsync(Guid offerId, Guid careHomeId, UpdateJobOfferDto dto);
        Task<bool> DeleteOfferAsync(Guid offerId, Guid careHomeId);

        Task<JobOfferDetailsDto?> GetOfferDetailsForPswAsync(Guid offerId);
        Task<Pagination<JobOffer>> GetOffersAsync(BaseQueryParams query);
    }
}
