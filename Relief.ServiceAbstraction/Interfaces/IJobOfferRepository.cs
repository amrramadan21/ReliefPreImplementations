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
        Task<JobOffer?> GetByIdAsync(Guid id);
        Task<List<JobOffer>> GetAllAsync();
        Task<List<JobOffer>> GetPagedAsync(int pageNumber, int pageSize);
        Task UpdateAsync();
        Task DeleteAsync(JobOffer offer);



    }
}
