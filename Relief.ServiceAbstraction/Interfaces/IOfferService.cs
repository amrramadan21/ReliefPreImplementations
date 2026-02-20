using Relief.Domain.Entities;
using Shared.OffersDTOs.CreateDTO;
using Shared.OffersDTOs.OfferInfoDTO;
using Shared.OffersDTOs.UpdateDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces
{
    public interface IOfferService
    {
        Task<Guid> CreateOfferAsync(Guid careHomeId, CreateJobOfferDto dto);
        Task<JobOfferDetailsDto?> GetOfferByIdAsync(Guid id);
        Task<List<JobOfferSummaryDto>> GetAllOffersAsync(int pageNumber,int pageSize);
        Task<bool> UpdateOfferAsync(Guid offerId, Guid careHomeId, UpdateJobOfferDto dto);
        Task<bool> DeleteOfferAsync(Guid offerId, Guid careHomeId);



    }
}
