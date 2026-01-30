using Shared.OffersDTOs.CreateDTO;
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
    }
}
