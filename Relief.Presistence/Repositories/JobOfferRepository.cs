using Relief.Domain.Entities;
using Relief.Presistence.Data.DbContexts;
using Relief.ServiceAbstraction.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presistence.Repositories
{
    public class JobOfferRepository :IJobOfferRepository
    {
        private readonly ReliefAppDbContext _context;

        public JobOfferRepository(ReliefAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobOffer offer)
        {
            await _context.JobOffers.AddAsync(offer);
            await _context.SaveChangesAsync();
        }
    }
}
