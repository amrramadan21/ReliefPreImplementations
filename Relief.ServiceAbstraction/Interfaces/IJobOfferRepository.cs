using Relief.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.ServiceAbstraction.Interfaces
{
    public interface IJobOfferRepository
    {
        Task AddAsync(JobOffer offer);
    }
}
