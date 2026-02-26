using E_CommerceLyaer.Presistence;
using Microsoft.EntityFrameworkCore;
using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Relief.Presistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presistence.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenaricRepository<TEntity, TKey> where TEntity : class
    {
        private readonly ReliefAppDbContext _dbContext;

        public GenericRepository(ReliefAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(TEntity entity) => await _dbContext.Set<TEntity>().AddAsync(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync() => await _dbContext.Set<TEntity>().ToListAsync();

        public async Task<TEntity?> GetByIdAsync(TKey id) => await _dbContext.Set<TEntity>().FindAsync(id);

        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);
        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationsEvaluator.CreateQuery(_dbContext.Set<TEntity>(), specifications).ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await SpecificationsEvaluator.CreateQuery(_dbContext.Set<TEntity>(), specifications).FirstOrDefaultAsync();
        }
    }
}
