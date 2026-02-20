using Microsoft.EntityFrameworkCore;
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
    public class JobOfferRepository : IJobOfferRepository
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

        public async Task<JobOffer?> GetByIdAsync(Guid id)
        {
            return await _context.JobOffers
                .Include(o => o.Days)
                .ThenInclude(d => d.Shifts)
                .FirstOrDefaultAsync(o => o.Id == id);
        }


        public async Task<List<JobOffer>> GetAllAsync()
        {
            return await _context.JobOffers
                .Include(o => o.Days)
                    .ThenInclude(d => d.Shifts)
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .ToListAsync();
        }

        public async Task<List<JobOffer>> GetPagedAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 5;

            return await _context.JobOffers
                .Include(o => o.Days)
                    .ThenInclude(d => d.Shifts)
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task UpdateAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(JobOffer offer)
        {
            _context.JobOffers.Remove(offer);
            await _context.SaveChangesAsync();
        }

    }
}
